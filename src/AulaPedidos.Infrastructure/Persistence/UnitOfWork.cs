using AulaPedidos.Application.Abstractions.Persistence;

namespace AulaPedidos.Infrastructure.Persistence;

/// <summary>Unidad de trabajo basada en <see cref="AppDbContext"/>.</summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
