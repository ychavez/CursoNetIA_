using AulaPedidos.Application.Abstractions.Persistence;
using AulaPedidos.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AulaPedidos.Infrastructure.Tests;

public sealed class InfrastructureCompositionTests
{
    [Fact]
    public void MissingConnectionStringThrowsAClearError()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            new ServiceCollection().AddInfrastructure(new ConfigurationBuilder().Build()));
        Assert.Contains("ConnectionStrings:Default", exception.Message, StringComparison.Ordinal);

        var empty = Assert.Throws<InvalidOperationException>(() =>
            new ServiceCollection().AddInfrastructure(Configuration(" ")));
        Assert.Contains("ConnectionStrings:Default", empty.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void RepositoryAndUnitOfWorkShareTheScopedContext()
    {
        var services = new ServiceCollection();
        Assert.Same(services, services.AddInfrastructure(Configuration("Data Source=:memory:")));
        using var provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = true
        });
        using var scope = provider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository<TestEntity>>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.IsType<Repository<TestEntity>>(repository);
        Assert.IsType<UnitOfWork>(unitOfWork);
        Assert.Same(repository, scope.ServiceProvider.GetRequiredService<IRepository<TestEntity>>());
        Assert.Same(context, scope.ServiceProvider.GetRequiredService<AppDbContext>());
        using var otherScope = provider.CreateScope();
        Assert.NotSame(context, otherScope.ServiceProvider.GetRequiredService<AppDbContext>());
    }

    private static IConfiguration Configuration(string connectionString) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = connectionString
            })
            .Build();
}
