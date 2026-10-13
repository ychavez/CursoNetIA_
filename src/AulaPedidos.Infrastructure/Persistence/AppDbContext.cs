using AulaPedidos.Domain;
using Microsoft.EntityFrameworkCore;

namespace AulaPedidos.Infrastructure.Persistence;

/// <summary>Contexto de persistencia de la aplicación.</summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        var product = modelBuilder.Entity<Product>();
        product.ToTable("Products");
        product.HasKey(product => product.Id);
        product.Property(product => product.Id).ValueGeneratedNever();
        product.Property(product => product.Price).HasColumnType("TEXT");
        product.Property(product => product.Description);
        product.Property(product => product.AlternateDescription);
    }
}
