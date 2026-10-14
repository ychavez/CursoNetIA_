using AulaPedidos.Api;
using AulaPedidos.Application;
using AulaPedidos.Infrastructure;
using AulaPedidos.Infrastructure.Persistence;
using Mediator;
using Microsoft.EntityFrameworkCore;

var migrate = args.Contains("--migrate", StringComparer.Ordinal);
var builder = WebApplication.CreateBuilder(args.Where(argument => argument != "--migrate").ToArray());

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddMediator(options =>
{
    options.Assemblies = [typeof(AulaPedidos.Application.DependencyInjection)];
    options.ServiceLifetime = ServiceLifetime.Scoped;
});
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddProblemDetails();

await using var app = builder.Build();

// Aplicación del esquema explícita y separada del arranque HTTP; nunca recrea la base.
if (migrate)
{
    await using var scope = app.Services.CreateAsyncScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync(app.Lifetime.ApplicationStopping);
    return;
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapHealthChecks("/health");
app.MapProducts();
app.Run();

public partial class Program;
