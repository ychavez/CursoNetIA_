using System.Linq.Expressions;
using AulaPedidos.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AulaPedidos.Infrastructure.Persistence;

/// <summary>Repositorio genérico sobre EF Core. Las escrituras requieren <see cref="IUnitOfWork"/>.</summary>
public sealed class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public Task<List<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default) =>
        Query(predicate).ToListAsync(cancellationToken);

    public Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default) =>
        Query(predicate).FirstOrDefaultAsync(cancellationToken);

    public Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default) =>
        Query(predicate).AnyAsync(cancellationToken);

    public Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default) =>
        Query(predicate).CountAsync(cancellationToken);

    public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>>? predicate = null)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>().AsNoTracking();
        return predicate is null ? query : query.Where(predicate);
    }

    public Task<List<TEntity>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, bool>>? predicate,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy,
        CancellationToken cancellationToken,
        params string[] includeStrings)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pageNumber, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

        var query = Query(predicate);
        foreach (var include in includeStrings ?? [])
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(include, nameof(includeStrings));
            query = query.Include(include);
        }

        if (orderBy is not null)
        {
            query = orderBy(query);
        }

        var skip = (long)(pageNumber - 1) * pageSize;
        if (skip > int.MaxValue)
        {
            return Task.FromResult(new List<TEntity>());
        }

        return query.Skip((int)skip).Take(pageSize).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _context.Set<TEntity>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
    }

    public void Update(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Set<TEntity>().Update(entity);
    }

    public void Remove(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Set<TEntity>().Remove(entity);
    }
}
