using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Xml.Linq;
using AulaPedidos.Application.Abstractions.Persistence;
using Xunit;

namespace AulaPedidos.ArchitectureTests;

public sealed class ArchitectureTests
{
    private static readonly string Root = FindRoot();
    private static readonly string[] ProductionProjects =
    ["AulaPedidos.Domain", "AulaPedidos.Application", "AulaPedidos.Infrastructure", "AulaPedidos.Api"];

    [Fact]
    public void SolutionContainsExactlyFourProductionAndTwoTestProjectsOnNet10()
    {
        var solution = XDocument.Load(Path.Combine(Root, "CursoNETIA.slnx"));
        var paths = solution.Descendants("Project").Select(project => (string)project.Attribute("Path")!).ToArray();
        var expected = ProductionProjects.Select(name => $"src/{name}/{name}.csproj")
            .Concat(new[]
            {
                "tests/AulaPedidos.ArchitectureTests/AulaPedidos.ArchitectureTests.csproj",
                "tests/AulaPedidos.Api.IntegrationTests/AulaPedidos.Api.IntegrationTests.csproj"
            });
        Assert.Equal(expected.Order(), paths.Order());
        foreach (var path in paths)
        {
            var project = XDocument.Load(Path.Combine(Root, path));
            Assert.Equal("net10.0", project.Descendants("TargetFramework").Single().Value);
        }
    }

    [Theory]
    [InlineData("AulaPedidos.Domain", new string[0], new string[0])]
    [InlineData("AulaPedidos.Application", new[] { "AulaPedidos.Domain" }, new[] { "Mediator.Abstractions", "Microsoft.Extensions.DependencyInjection.Abstractions" })]
    [InlineData("AulaPedidos.Infrastructure", new[] { "AulaPedidos.Application" }, new[] { "Microsoft.Extensions.Configuration.Abstractions", "Microsoft.Extensions.DependencyInjection.Abstractions" })]
    [InlineData("AulaPedidos.Api", new[] { "AulaPedidos.Application", "AulaPedidos.Infrastructure" }, new[] { "Mediator.SourceGenerator", "Microsoft.AspNetCore.OpenApi" })]
    public void ProjectReferencesAndPackagesRespectLayerBoundaries(string name, string[] projects, string[] packages)
    {
        var project = XDocument.Load(ProjectPath(name));
        var references = project.Descendants("ProjectReference")
            .Select(reference => Path.GetFileNameWithoutExtension(((string)reference.Attribute("Include")!).Replace('\\', '/')));
        Assert.Equal(projects.Order(), references.Order());
        Assert.Equal(packages.Order(), project.Descendants("PackageReference")
            .Select(reference => (string)reference.Attribute("Include")!).Order());
        Assert.Empty(project.Descendants("FrameworkReference"));
        if (name == "AulaPedidos.Api")
        {
            var generator = project.Descendants("PackageReference")
                .Single(reference => (string?)reference.Attribute("Include") == "Mediator.SourceGenerator");
            Assert.Equal("all", (string?)generator.Attribute("PrivateAssets"));
        }
    }

    [Theory]
    [InlineData("AulaPedidos.Domain", new string[0])]
    [InlineData("AulaPedidos.Application", new[] { "AulaPedidos.Domain" })]
    [InlineData("AulaPedidos.Infrastructure", new[] { "AulaPedidos.Application" })]
    [InlineData("AulaPedidos.Api", new[] { "AulaPedidos.Application", "AulaPedidos.Infrastructure" })]
    public void CompiledReferencesCannotCrossForbiddenBoundaries(string name, string[] allowed)
    {
        // An unused project reference need not appear in assembly metadata.
        var references = Assembly.Load(name).GetReferencedAssemblies().Select(reference => reference.Name!).ToArray();
        Assert.All(references.Where(reference => reference.StartsWith("AulaPedidos.", StringComparison.Ordinal)),
            reference => Assert.Contains(reference, allowed));
        if (name is "AulaPedidos.Domain" or "AulaPedidos.Application")
        {
            Assert.DoesNotContain(references, reference => reference.StartsWith("Microsoft.AspNetCore", StringComparison.Ordinal));
        }
        if (name == "AulaPedidos.Domain")
        {
            Assert.DoesNotContain(references, reference => reference.StartsWith("Microsoft.Extensions", StringComparison.Ordinal) || reference.StartsWith("Mediator", StringComparison.Ordinal));
        }
        if (name == "AulaPedidos.Application")
        {
            Assert.Contains("Mediator", references);
        }
    }

    [Theory]
    [InlineData("AulaPedidos.Domain")]
    [InlineData("AulaPedidos.Application")]
    [InlineData("AulaPedidos.Infrastructure")]
    [InlineData("AulaPedidos.Api")]
    public void RestoredGraphHasNoPersistenceOrUnrequestedMessagingPackages(string name)
    {
        using var assets = JsonDocument.Parse(File.ReadAllText(Path.Combine(Root, "src", name, "obj", "project.assets.json")));
        var packages = assets.RootElement.GetProperty("libraries").EnumerateObject()
            .Where(library => library.Value.GetProperty("type").GetString() == "package")
            .Select(library => library.Name.Split('/')[0]).ToArray();
        Assert.DoesNotContain(packages, package =>
            package.StartsWith("MediatR", StringComparison.OrdinalIgnoreCase) ||
            package.Contains("EntityFramework", StringComparison.OrdinalIgnoreCase) ||
            package.Contains("SqlClient", StringComparison.OrdinalIgnoreCase) ||
            package.Contains("Sqlite", StringComparison.OrdinalIgnoreCase) ||
            package.Contains("Npgsql", StringComparison.OrdinalIgnoreCase) ||
            package.Contains("MySql", StringComparison.OrdinalIgnoreCase) ||
            package.Contains("MassTransit", StringComparison.OrdinalIgnoreCase) ||
            package.Contains("Wolverine", StringComparison.OrdinalIgnoreCase));
        if (name == "AulaPedidos.Api")
        {
            Assert.Contains("Mediator.Abstractions", packages);
            Assert.Contains("Mediator.SourceGenerator", packages);
        }
    }

    [Fact]
    public void RepositoryContractIsMinimalAndStorageIndependent()
    {
        var repository = typeof(IRepository<>);
        Assert.Equal("AulaPedidos.Application", repository.Assembly.GetName().Name);
        Assert.Equal("AulaPedidos.Application.Abstractions.Persistence", repository.Namespace);
        Assert.True(repository.IsInterface);
        Assert.Empty(repository.GetInterfaces());
        Assert.Empty(repository.GetProperties());
        var entityType = repository.GetGenericArguments().Single();
        Assert.Empty(entityType.GetGenericParameterConstraints());
        Assert.Equal(GenericParameterAttributes.ReferenceTypeConstraint, entityType.GenericParameterAttributes);
        Assert.Equal(new[] { "AddAsync", "RemoveAsync" }, repository.GetMethods().Select(method => method.Name).Order());
        foreach (var method in repository.GetMethods())
        {
            Assert.Equal(typeof(Task), method.ReturnType);
            var parameters = method.GetParameters();
            Assert.Equal(2, parameters.Length);
            Assert.Equal(entityType, parameters[0].ParameterType);
            Assert.Equal(typeof(CancellationToken), parameters[1].ParameterType);
            Assert.True(parameters[1].IsOptional);
        }
    }

    [Fact]
    public void ProductionContainsOnlyTechnicalTypesAndNoRepositoryImplementation()
    {
        Assert.Empty(DeclaredTypes("AulaPedidos.Domain"));
        Assert.Equal(new[]
        {
            "AulaPedidos.Application.Abstractions.Persistence.IRepository`1",
            "AulaPedidos.Application.DependencyInjection"
        }, DeclaredTypes("AulaPedidos.Application").Select(type => type.FullName!).Order());
        Assert.Equal(new[] { "AulaPedidos.Infrastructure.DependencyInjection" },
            DeclaredTypes("AulaPedidos.Infrastructure").Select(type => type.FullName!));
        Assert.All(DeclaredTypes("AulaPedidos.Api"), type =>
            Assert.True(type == typeof(Program) || type.Namespace is "Mediator" or "Mediator.Internals" ||
                type.FullName == "Microsoft.Extensions.DependencyInjection.MediatorDependencyInjectionExtensions", type.FullName));
        var apiDirectory = Path.Combine(Root, "src", "AulaPedidos.Api");
        Assert.Equal(new[] { "Program.cs" }, Directory.EnumerateFiles(apiDirectory, "*.cs", SearchOption.AllDirectories)
            .Select(path => Path.GetRelativePath(apiDirectory, path))
            .Where(path => !path.StartsWith($"obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal) &&
                !path.StartsWith($"bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)).Order());
        foreach (var type in ProductionProjects.SelectMany(DeclaredTypes))
        {
            Assert.DoesNotContain(type.GetInterfaces(), contract =>
                contract.IsGenericType && contract.GetGenericTypeDefinition() == typeof(IRepository<>));
            Assert.DoesNotContain(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static),
                field => field.FieldType.IsGenericType &&
                    field.FieldType.GetGenericArguments().Any(argument => argument.Namespace?.StartsWith("AulaPedidos.", StringComparison.Ordinal) == true));
        }
    }

    [Fact]
    public void ApiConfigurationHasNoDatabaseOrBusinessSettings()
    {
        foreach (var file in Directory.EnumerateFiles(Path.Combine(Root, "src", "AulaPedidos.Api"), "appsettings*.json"))
        {
            using var document = JsonDocument.Parse(File.ReadAllText(file));
            Assert.All(document.RootElement.EnumerateObject(), property =>
                Assert.Contains(property.Name, new[] { "Logging", "AllowedHosts" }));
        }
        var httpFile = File.ReadAllLines(Path.Combine(Root, "src", "AulaPedidos.Api", "AulaPedidos.Api.http"));
        Assert.Equal(new[] { "GET {{AulaPedidos.Api_HostAddress}}/health", "GET {{AulaPedidos.Api_HostAddress}}/openapi/v1.json" },
            httpFile.Where(line => line.StartsWith("GET ", StringComparison.Ordinal)));
    }

    private static IEnumerable<Type> DeclaredTypes(string name) => Assembly.Load(name).GetTypes()
        .Where(type => !IsCompilerGenerated(type) &&
            type.FullName != "Microsoft.AspNetCore.OpenApi.Generated.OpenApiJsonSchemaContext");

    private static bool IsCompilerGenerated(Type type)
    {
        for (Type? current = type; current is not null; current = current.DeclaringType)
        {
            if (current.IsDefined(typeof(CompilerGeneratedAttribute), false) ||
                current.Name.StartsWith("<", StringComparison.Ordinal))
            {
                return true;
            }
        }
        return false;
    }

    private static string ProjectPath(string name) => Path.Combine(Root, "src", name, $"{name}.csproj");

    private static string FindRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "CursoNETIA.slnx")))
            {
                return directory.FullName;
            }
        }
        throw new DirectoryNotFoundException("No se encontró CursoNETIA.slnx desde el directorio de pruebas.");
    }
}