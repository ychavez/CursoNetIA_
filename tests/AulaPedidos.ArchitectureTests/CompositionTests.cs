using AulaPedidos.Application;
using AulaPedidos.Application.Abstractions.Persistence;
using AulaPedidos.Infrastructure;
using Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AulaPedidos.ArchitectureTests;

public sealed class CompositionTests
{
    [Fact]
    public void ApplicationRejectsNullServices()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            AulaPedidos.Application.DependencyInjection.AddApplication(null!));
        Assert.Equal("services", exception.ParamName);
    }

    [Fact]
    public void InfrastructureRejectsNullServices()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            AulaPedidos.Infrastructure.DependencyInjection.AddInfrastructure(null!, new ConfigurationBuilder().Build()));
        Assert.Equal("services", exception.ParamName);
    }

    [Fact]
    public void InfrastructureRejectsNullConfiguration()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new ServiceCollection().AddInfrastructure(null!));
        Assert.Equal("configuration", exception.ParamName);
    }

    [Fact]
    public void CompositionIsChainableWithoutStorageAndDoesNotRegisterRepositories()
    {
        var services = new ServiceCollection();
        Assert.Same(services, services.AddApplication());
        var beforeInfrastructure = services.ToArray();
        Assert.Same(services, services.AddInfrastructure(new ConfigurationBuilder().Build()));
        Assert.Equal(beforeInfrastructure, services.ToArray());
        Assert.Equal(new[] { typeof(IPublisher), typeof(ISender) }.OrderBy(type => type.Name),
            services.Select(descriptor => descriptor.ServiceType).OrderBy(type => type.Name));
        Assert.All(services, descriptor => Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime));
        using var provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = true
        });
        Assert.Null(provider.GetService<IRepository<RepositoryProbe>>());
        Assert.Empty(provider.GetServices<IRepository<RepositoryProbe>>());
        Assert.Null(provider.GetService(typeof(IRepository<>)));
    }

    [Fact]
    public void ApplicationRegistrationIsIdempotent()
    {
        var services = new ServiceCollection();
        services.AddApplication();
        var original = services.ToArray();
        services.AddApplication();
        Assert.Equal(original, services.ToArray());
    }

    private sealed class RepositoryProbe;
}