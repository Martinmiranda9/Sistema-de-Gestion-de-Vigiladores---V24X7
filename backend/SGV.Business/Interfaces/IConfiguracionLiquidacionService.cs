using SGV.DTOs.ConfiguracionLiquidacion;

namespace SGV.Business.Interfaces;

public interface IConfiguracionLiquidacionService
{
    Task<IEnumerable<ConfiguracionLiquidacionDTO>> GetAllAsync();
    Task<ConfiguracionLiquidacionDTO?> GetByIdAsync(int id);
    Task<ConfiguracionLiquidacionDTO> CreateAsync(ConfiguracionLiquidacionCreateDTO dto);
    Task<ConfiguracionLiquidacionDTO?> UpdateAsync(int id, ConfiguracionLiquidacionUpdateDTO dto);
    Task<bool> DeleteAsync(int id);
    Task<ConfiguracionLiquidacionDTO?> GetVigenteAsync(DateTime fecha);
}
