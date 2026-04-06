using SGV.Entities;

namespace SGV.Business.Interfaces.Repositories;

/// <summary>
/// Contrato de acceso a datos para la entidad ConfiguracionLiquidacion.
/// </summary>
public interface IConfiguracionLiquidacionRepository
{
    /// <summary>Obtiene todas las configuraciones de liquidación, ordenadas de más reciente a más antigua.</summary>
    Task<IEnumerable<ConfiguracionLiquidacion>> GetAllAsync();

    /// <summary>Obtiene una configuración por Id. Null si no existe.</summary>
    Task<ConfiguracionLiquidacion?> GetByIdAsync(int id);

    /// <summary>
    /// Obtiene la configuración de precios vigente para una fecha dada.
    /// Devuelve la más reciente cuya FechaDesde sea menor o igual a la fecha consultada.
    /// Null si no hay ninguna configuración que aplique.
    /// </summary>
    Task<ConfiguracionLiquidacion?> GetVigenteAsync(DateTime fecha);

    /// <summary>Agrega una nueva configuración de liquidación al repositorio.</summary>
    Task AddAsync(ConfiguracionLiquidacion config);

    /// <summary>Elimina físicamente una configuración del repositorio.</summary>
    void Remove(ConfiguracionLiquidacion config);

    /// <summary>Persiste los cambios realizados sobre una entidad rastreada.</summary>
    Task SaveChangesAsync();
}
