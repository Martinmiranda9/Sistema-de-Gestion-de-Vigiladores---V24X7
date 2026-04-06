using SGV.Entities;

namespace SGV.Business.Interfaces.Repositories;

/// <summary>
/// Contrato de acceso a datos para la entidad Vigilador.
/// Definido en la capa Business para mantener independencia de infraestructura.
/// </summary>
public interface IVigiladorRepository
{
    /// <summary>Obtiene todos los vigiladores, incluyendo su Objetivo asignado.</summary>
    Task<IEnumerable<Vigilador>> GetAllAsync();

    /// <summary>Obtiene un vigilador por Id, incluyendo su Objetivo asignado. Null si no existe.</summary>
    Task<Vigilador?> GetByIdAsync(int id);

    /// <summary>Obtiene todos los vigiladores activos asignados a un Objetivo específico.</summary>
    Task<IEnumerable<Vigilador>> GetByObjetivoAsync(int objetivoId);

    /// <summary>Verifica si ya existe un vigilador con el DNI dado (excluyendo un Id opcional para updates).</summary>
    Task<bool> ExistsByDNIAsync(string dni, int? excludeId = null);

    /// <summary>Agrega un nuevo vigilador al repositorio.</summary>
    Task AddAsync(Vigilador vigilador);

    /// <summary>Persiste los cambios realizados sobre una entidad rastreada.</summary>
    Task SaveChangesAsync();
}
