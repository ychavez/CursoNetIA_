using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AulaPedidos.Api;
using AulaPedidos.Application.Products;
using AulaPedidos.Application.Validation;
using AulaPedidos.Infrastructure.Persistence;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace AulaPedidos.Api.IntegrationTests;

public sealed class ValidationApiTests
{
    [Theory]
    [InlineData("application/json")]
    [InlineData("application/xml")]
    public async Task RealPipelineRejectsCreateAndUpdateWithoutPersistingAndGroupsSafeErrors(string accept)
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var createCalls = 0;
        var first = new InlineValidator<CreateProductCommand>();
        first.RuleFor(command => command).Custom((_, context) =>
        {
            createCalls++;
            context.AddFailure("Price", "E1");
            context.AddFailure("Description", "E2");
        });
        var second = new InlineValidator<CreateProductCommand>();
        second.RuleFor(command => command.Price).Custom((_, context) => context.AddFailure("E3"));
        var update = new InlineValidator<UpdateProductCommand>();
        update.RuleFor(command => command.Price).Custom((_, context) => context.AddFailure("Edición rechazada"));
        await using var factory = CreateFactory(connection, services =>
        {
            services.AddScoped<IValidator<CreateProductCommand>>(_ => first);
            services.AddScoped<IValidator<CreateProductCommand>>(_ => second);
            services.AddScoped<IValidator<UpdateProductCommand>>(_ => update);
        });
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Accept.ParseAdd(accept);
        Guid existingId;
        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Database.MigrateAsync();
            var product = new AulaPedidos.Domain.Product(Guid.NewGuid(), 10, "Original", null);
            existingId = product.Id;
            context.Add(product);
            await context.SaveChangesAsync();
        }

        using var response = await client.PostAsJsonAsync("/products", new
        {
            price = 12.5m, description = "SENTINEL_PRIVATE", alternateDescription = (string?)null
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);
        Assert.Equal(400, document.RootElement.GetProperty("status").GetInt32());
        var errors = document.RootElement.GetProperty("errors");
        Assert.Equal(["E1", "E3"], errors.GetProperty("Price").EnumerateArray().Select(error => error.GetString()));
        Assert.Equal("E2", Assert.Single(errors.GetProperty("Description").EnumerateArray()).GetString());
        Assert.DoesNotContain("SENTINEL_PRIVATE", body);
        Assert.DoesNotContain("ValidationException", body);
        Assert.Equal(1, createCalls);

        using var rejectedUpdate = await client.PutAsJsonAsync($"/products/{existingId}",
            new { price = 99, description = "Modificado" });
        Assert.Equal(HttpStatusCode.BadRequest, rejectedUpdate.StatusCode);
        await using var verificationScope = factory.Services.CreateAsyncScope();
        var stored = Assert.Single(await verificationScope.ServiceProvider.GetRequiredService<AppDbContext>()
            .Set<AulaPedidos.Domain.Product>().AsNoTracking().ToListAsync());
        Assert.Equal(existingId, stored.Id);
        Assert.Equal(10, stored.Price);
        Assert.Equal("Original", stored.Description);
    }

    [Fact]
    public async Task RealSenderValidatesQueriesAndResolvesValidatorsPerScopeWithoutLeakingFailures()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var factory = CreateFactory(connection, services =>
            services.AddScoped<IValidator<GetProductsQuery>, ScopedQueryValidator>());
        using var client = factory.CreateClient();
        await using var firstScope = factory.Services.CreateAsyncScope();
        var context = firstScope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
        var firstValidator = Assert.IsType<ScopedQueryValidator>(
            firstScope.ServiceProvider.GetRequiredService<IValidator<GetProductsQuery>>());
        var firstSender = firstScope.ServiceProvider.GetRequiredService<ISender>();
        Assert.IsType<ValidationBehavior<GetProductsQuery, IReadOnlyList<ProductDto>>>(
            firstScope.ServiceProvider.GetRequiredService<IPipelineBehavior<GetProductsQuery, IReadOnlyList<ProductDto>>>());
        await Assert.ThrowsAsync<ValidationException>(() => firstSender.Send(new GetProductsQuery()).AsTask());
        firstValidator.Reject = false;
        Assert.Empty(await firstSender.Send(new GetProductsQuery()));
        Assert.Equal(2, firstValidator.Calls);

        await using var secondScope = factory.Services.CreateAsyncScope();
        var secondValidator = Assert.IsType<ScopedQueryValidator>(
            secondScope.ServiceProvider.GetRequiredService<IValidator<GetProductsQuery>>());
        Assert.NotSame(firstValidator, secondValidator);
        await Assert.ThrowsAsync<ValidationException>(() =>
            secondScope.ServiceProvider.GetRequiredService<ISender>().Send(new GetProductsQuery()).AsTask());
        Assert.Equal(1, secondValidator.Calls);

        using var response = await client.GetAsync("/products");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Consulta rechazada", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task UnexpectedValidatorFailureRemainsGeneric500InProduction()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        var validator = new InlineValidator<GetProductsQuery>();
        validator.RuleFor(query => query).Custom((_, _) => throw new InvalidOperationException("TECHNICAL_SENTINEL"));
        await using var factory = CreateFactory(connection, services =>
            services.AddScoped<IValidator<GetProductsQuery>>(_ => validator));
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/products");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("TECHNICAL_SENTINEL", body);
        Assert.DoesNotContain("InvalidOperationException", body);
        Assert.DoesNotContain("errors", body);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ExceptionTranslatorDoesNotHandleCancellationOrTechnicalFailures(bool cancelled)
    {
        var services = new ServiceCollection().AddLogging().AddProblemDetails().BuildServiceProvider();
        await using var provider = services;
        var handler = new ValidationExceptionHandler(provider.GetRequiredService<IProblemDetailsService>());
        var context = new DefaultHttpContext();
        Exception exception = cancelled ? new OperationCanceledException() : new InvalidOperationException();

        Assert.False(await handler.TryHandleAsync(context, exception, default));
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    private static WebApplicationFactory<Program> CreateFactory(SqliteConnection connection, Action<IServiceCollection> configure) =>
        new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Production");
            builder.UseDefaultServiceProvider(options =>
            {
                options.ValidateScopes = true;
                options.ValidateOnBuild = true;
            });
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<AppDbContext>>();
                services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
                services.AddDbContext<AppDbContext>(options => options.UseSqlite(connection));
                configure(services);
            });
        });

    private sealed class ScopedQueryValidator : AbstractValidator<GetProductsQuery>
    {
        public bool Reject { get; set; } = true;
        public int Calls { get; private set; }

        public ScopedQueryValidator()
        {
            RuleFor(query => query).Custom((_, context) =>
            {
                Calls++;
                if (Reject) context.AddFailure("Query", "Consulta rechazada");
            });
        }
    }
}
