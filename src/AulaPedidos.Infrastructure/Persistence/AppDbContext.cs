using Microsoft.EntityFrameworkCore;

namespace AulaPedidos.Infrastructure.Persistence;

/// <summary>Contexto de EF Core de la aplicación. Aún no declara entidades de negocio.</summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}
