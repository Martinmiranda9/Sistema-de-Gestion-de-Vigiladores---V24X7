using SGV.Entities;

namespace SGV.Business.Interfaces.Repositories;

/// <summary>
/// Contrato de acceso a datos para la entidad Objetivo (lugar de trabajo).
/// </summary>
public interface IObjetivoRepository
{
    /// <summary>Obtiene todos los objetivos. Por defecto solo los activos.</summary>
    Task<IEnumerable<Objetivo>> GetAllAsync(bool incluirInactivos = false);

    /// <summary>Obtiene un objetivo por Id, activo o no. Null si no existe.</summary>
    Task<Objetivo?> GetByIdAsync(int id);

    /// <summary>Agrega un nuevo objetivo al repositorio.</summary>
    Task AddAsync(Objetivo objetivo);

    /// <summary>Persiste los cambios realizados sobre una entidad rastreada.</summary>
    Task SaveChangesAsync();
}
