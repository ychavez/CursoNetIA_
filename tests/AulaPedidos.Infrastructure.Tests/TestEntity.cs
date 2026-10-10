using Microsoft.EntityFrameworkCore;

namespace AulaPedidos.Infrastructure.Tests;

/// <summary>Entidad usada solo por las pruebas; no forma parte del dominio.</summary>
public sealed class TestEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Value { get; set; }

    public TestChild? Child { get; set; }
}

/// <summary>Navegación usada para comprobar los includes por nombre.</summary>
public sealed class TestChild
{
    public int Id { get; set; }

    public string Label { get; set; } = string.Empty;
}

/// <summary>Contexto de pruebas que añade la entidad de test al modelo.</summary>
public sealed class TestDbContext : Persistence.AppDbContext
{
    public TestDbContext(DbContextOptions<Persistence.AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<TestEntity>();
        modelBuilder.Entity<TestChild>();
    }
}
