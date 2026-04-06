using SGV.Entities;

namespace SGV.Business.Interfaces.Repositories;

/// <summary>
/// Contrato de acceso a datos para la entidad RegistroTurno.
/// </summary>
public interface IRegistroTurnoRepository
{
    /// <summary>Obtiene todos los registros de turno, incluyendo el Vigilador asociado.</summary>
    Task<IEnumerable<RegistroTurno>> GetAllAsync();

    /// <summary>Obtiene un registro de turno por Id, incluyendo el Vigilador asociado. Null si no existe.</summary>
    Task<RegistroTurno?> GetByIdAsync(int id);

    /// <summary>
    /// Obtiene los turnos de un vigilador filtrados por mes y año.
    /// Útil para generar el almanaque mensual.
    /// </summary>
    Task<IEnumerable<RegistroTurno>> GetByVigiladorAsync(int vigiladorId, int mes, int anio);

    /// <summary>Agrega un nuevo registro de turno al repositorio.</summary>
    Task AddAsync(RegistroTurno registro);

    /// <summary>Elimina físicamente un registro de turno del repositorio.</summary>
    void Remove(RegistroTurno registro);

    /// <summary>Persiste los cambios realizados sobre una entidad rastreada.</summary>
    Task SaveChangesAsync();
}
