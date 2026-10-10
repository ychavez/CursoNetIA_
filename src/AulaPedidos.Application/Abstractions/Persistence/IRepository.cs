using System.Linq.Expressions;

namespace AulaPedidos.Application.Abstractions.Persistence;

/// <summary>
/// Contrato de acceso a datos independiente del almacenamiento.
/// Las escrituras no se persisten hasta llamar a <see cref="IUnitOfWork.SaveChangesAsync"/>.
/// </summary>
/// <typeparam name="TEntity">Tipo de la entidad administrada.</typeparam>
public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>Devuelve las entidades que cumplen el predicado; sin predicado devuelve todas.</summary>
    Task<List<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    /// <summary>Devuelve la primera entidad que cumple el predicado o <c>null</c>.</summary>
    Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    /// <summary>Indica si existe alguna entidad que cumpla el predicado.</summary>
    Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    /// <summary>Cuenta las entidades que cumplen el predicado.</summary>
    Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Devuelve una consulta de solo lectura y sin seguimiento para componer operaciones adicionales.
    /// La consulta no se ejecuta hasta materializarla.
    /// </summary>
    IQueryable<TEntity> Query(Expression<Func<TEntity, bool>>? predicate = null);

    /// <summary>Devuelve una página de resultados.</summary>
    /// <param name="pageNumber">Número de página, empezando en 1.</param>
    /// <param name="pageSize">Tamaño de página, mayor o igual que 1.</param>
    /// <param name="predicate">Filtro opcional.</param>
    /// <param name="orderBy">
    /// Orden opcional. Sin un orden explícito la paginación no es determinista,
    /// porque el almacenamiento no garantiza un orden estable entre páginas.
    /// </param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <param name="includeStrings">Rutas de navegación que se incluyen en la consulta.</param>
    Task<List<TEntity>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, bool>>? predicate,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy,
        CancellationToken cancellationToken,
        params string[] includeStrings);

    /// <summary>Marca la entidad para inserción; no persiste.</summary>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>Marca la entidad como modificada; no persiste.</summary>
    void Update(TEntity entity);

    /// <summary>Marca la entidad para eliminación; no persiste.</summary>
    void Remove(TEntity entity);
}
