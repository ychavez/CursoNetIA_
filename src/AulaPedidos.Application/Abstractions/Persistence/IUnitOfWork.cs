namespace AulaPedidos.Application.Abstractions.Persistence;

/// <summary>Confirma en el almacenamiento los cambios pendientes del ámbito actual.</summary>
public interface IUnitOfWork
{
    /// <summary>Persiste los cambios pendientes y devuelve el número de filas afectadas.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
