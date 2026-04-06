using SGV.Entities;

namespace SGV.Business.Interfaces.Repositories;

/// <summary>
/// Contrato de acceso a datos para la entidad Feriado.
/// </summary>
public interface IFeriadoRepository
{
    /// <summary>Obtiene todos los feriados, ordenados por fecha.</summary>
    Task<IEnumerable<Feriado>> GetAllAsync();

    /// <summary>Obtiene un feriado por Id. Null si no existe.</summary>
    Task<Feriado?> GetByIdAsync(int id);

    /// <summary>
    /// Obtiene los feriados de un año dado.
    /// Incluye feriados recurrentes (que coinciden en día y mes independientemente del año).
    /// </summary>
    Task<IEnumerable<Feriado>> GetByAnioAsync(int anio);

    /// <summary>
    /// Verifica si una fecha dada cae en un feriado.
    /// Compara por fecha exacta o por recurrencia anual (mismo día y mes).
    /// </summary>
    Task<bool> EsFeriadoAsync(DateTime fecha);

    /// <summary>Agrega un nuevo feriado al repositorio.</summary>
    Task AddAsync(Feriado feriado);

    /// <summary>Elimina físicamente un feriado del repositorio.</summary>
    void Remove(Feriado feriado);

    /// <summary>Persiste los cambios realizados sobre una entidad rastreada.</summary>
    Task SaveChangesAsync();
}
