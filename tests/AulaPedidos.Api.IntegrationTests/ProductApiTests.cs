using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AulaPedidos.Application.Products;
using AulaPedidos.Domain;
using AulaPedidos.Infrastructure.Persistence;
using Mediator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace AulaPedidos.Api.IntegrationTests;

public sealed class ProductApiTests : IAsyncLifetime
{
    private readonly string _databasePath = Path.Join(FindRoot(), "TestResults", $"products-{Guid.NewGuid():N}.db");
    private WebApplicationFactory<Program> _baseFactory = null!;
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_databasePath)!);
        _baseFactory = new WebApplicationFactory<Program>();
        _factory = _baseFactory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Production");
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<AppDbContext>>();
                services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
                services.AddDbContext<AppDbContext>(options => options.UseSqlite($"Data Source={_databasePath};Pooling=False"));
            });
            builder.UseDefaultServiceProvider(options =>
            {
                options.ValidateScopes = true;
                options.ValidateOnBuild = true;
            });
        });
        _client = _factory.CreateClient();
        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal(_databasePath, context.Database.GetDbConnection().DataSource);
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _baseFactory.DisposeAsync();
        File.Delete(_databasePath);
        File.Delete(_databasePath + "-wal");
        File.Delete(_databasePath + "-shm");
    }

    [Fact]
    public async Task CrudPersistsAcrossScopesAndDoesNotChangeOtherProducts()
    {
        Assert.Empty((await _client.GetFromJsonAsync<ProductDto[]>("/products"))!);
        var first = await CreateAsync(123.456789m, " descripción ", "alternativa");
        var other = await CreateAsync(0, null, null);
        Assert.NotEqual(Guid.Empty, first.Id);
        Assert.NotEqual(first.Id, other.Id);
        Assert.Equal(first, await ReadStoredAsync(first.Id));
        Assert.Equal(first, await _client.GetFromJsonAsync<ProductDto>($"/products/{first.Id}"));
        var listed = (await _client.GetFromJsonAsync<ProductDto[]>("/products"))!;
        Assert.Equal(new[] { first, other }.OrderBy(product => product.Id), listed);

        using var update = await _client.PutAsJsonAsync($"/products/{first.Id}",
            new AulaPedidos.Api.ProductRequest(-9.25m, null, " nueva "));
        Assert.Equal(HttpStatusCode.OK, update.StatusCode);
        var expected = new ProductDto(first.Id, -9.25m, null, " nueva ");
        Assert.Equal(expected, await update.Content.ReadFromJsonAsync<ProductDto>());
        Assert.Equal(expected, await ReadStoredAsync(first.Id));
        Assert.Equal(other, await ReadStoredAsync(other.Id));
        Assert.Equal(2, (await _client.GetFromJsonAsync<ProductDto[]>("/products"))!.Length);

        using var delete = await _client.DeleteAsync($"/products/{first.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
        Assert.Empty(await delete.Content.ReadAsByteArrayAsync());
        Assert.Null(await ReadStoredAsync(first.Id));
        using var missing = await _client.GetAsync($"/products/{first.Id}");
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
        Assert.Equal(other, Assert.Single((await _client.GetFromJsonAsync<ProductDto[]>("/products"))!));
    }

    [Theory]
    [InlineData("get")]
    [InlineData("put")]
    [InlineData("delete")]
    public async Task MissingOrMalformedIdentifiersReturn404WithoutWrites(string method)
    {
        foreach (var id in new[] { Guid.NewGuid().ToString(), Guid.Empty.ToString(), "not-a-guid" })
        {
            using var request = new HttpRequestMessage(new HttpMethod(method), $"/products/{id}");
            if (method == "put")
            {
                request.Content = JsonContent.Create(new { price = 2, description = "test" });
            }

            using var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        Assert.Empty((await _client.GetFromJsonAsync<ProductDto[]>("/products"))!);
    }

    [Theory]
    [InlineData("{")]
    [InlineData("null")]
    [InlineData("{}")]
    [InlineData("{\"price\":null}")]
    [InlineData("{\"price\":\"invalid\"}")]
    [InlineData("{\"price\":79228162514264337593543950336}")]
    [InlineData("{\"price\":1,\"description\":42}")]
    public async Task InvalidJsonCannotCreateOrPartiallyUpdate(string json)
    {
        var stored = await CreateAsync(10m, "original", "original alterna");
        var requests = new[] { "/products", $"/products/{stored.Id}" }.Select(path =>
            new HttpRequestMessage(path == "/products" ? HttpMethod.Post : HttpMethod.Put, path)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });
        foreach (var request in requests)
        {
            using (request)
            {
                using var response = await _client.SendAsync(request);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                Assert.DoesNotContain("Exception", await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);
            }
        }

        Assert.Equal(stored, await ReadStoredAsync(stored.Id));
        Assert.Single((await _client.GetFromJsonAsync<ProductDto[]>("/products"))!);
    }

    [Theory]
    [InlineData("0", "", "  ")]
    [InlineData("-0.01", null, null)]
    [InlineData("79228162514264337593543950335", "á漢字", "alternativa")]
    [InlineData("-79228162514264337593543950335", "  descripción  ", "")]
    public async Task DecimalBoundariesAndTextsRoundTripWithoutCommercialRestrictions(
        string priceText, string? description, string? alternateDescription)
    {
        var price = decimal.Parse(priceText, System.Globalization.CultureInfo.InvariantCulture);
        var product = await CreateAsync(price, description, alternateDescription);
        Assert.Equal(new ProductDto(product.Id, price, description, alternateDescription), await ReadStoredAsync(product.Id));
    }

    [Fact]
    public async Task InputCannotChooseOrReplaceTheIdentityAndContractHasOnlyRequestedFields()
    {
        var suppliedId = Guid.NewGuid();
        using var response = await _client.PostAsJsonAsync("/products", new { id = suppliedId, price = 1 });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(new[] { "alternateDescription", "description", "id", "price" },
            document.RootElement.EnumerateObject().Select(property => property.Name).Order());
        Assert.NotEqual(suppliedId, document.RootElement.GetProperty("id").GetGuid());
    }

    [Fact]
    public async Task RealMediatorHonoursCancellationBeforeReadsAndWrites()
    {
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();
        await using var scope = _factory.Services.CreateAsyncScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await sender.Send(new CreateProductCommand(1, "cancelled", null), cancellation.Token));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await sender.Send(new GetProductsQuery(), cancellation.Token));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await sender.Send(new GetProductByIdQuery(Guid.NewGuid()), cancellation.Token));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await sender.Send(new UpdateProductCommand(Guid.NewGuid(), 1, null, null), cancellation.Token));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await sender.Send(new DeleteProductCommand(Guid.NewGuid()), cancellation.Token));
        Assert.Empty((await _client.GetFromJsonAsync<ProductDto[]>("/products"))!);
    }

    private async Task<ProductDto> CreateAsync(decimal price, string? description, string? alternateDescription)
    {
        using var response = await _client.PostAsJsonAsync("/products", new { price, description, alternateDescription });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var product = (await response.Content.ReadFromJsonAsync<ProductDto>())!;
        Assert.Equal($"/products/{product.Id}", response.Headers.Location?.ToString());
        using var get = await _client.GetAsync(response.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
        return product;
    }

    private async Task<ProductDto?> ReadStoredAsync(Guid id)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var product = await context.Set<Product>().AsNoTracking().SingleOrDefaultAsync(product => product.Id == id);
        Assert.Empty(context.ChangeTracker.Entries());
        return product is null ? null : ProductDto.FromProduct(product);
    }

    private static string FindRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Join(directory.FullName, "CursoNETIA.slnx")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName ?? throw new InvalidOperationException("No se encuentra la solución.");
    }
}
