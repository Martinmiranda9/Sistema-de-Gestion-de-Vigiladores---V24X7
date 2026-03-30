using SGV.DTOs.Feriado;

namespace SGV.Business.Interfaces;

public interface IFeriadoService
{
    Task<IEnumerable<FeriadoDTO>> GetAllAsync();
    Task<FeriadoDTO?> GetByIdAsync(int id);
    Task<FeriadoDTO> CreateAsync(FeriadoCreateDTO dto);
    Task<FeriadoDTO?> UpdateAsync(int id, FeriadoUpdateDTO dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<FeriadoDTO>> GetByAnioAsync(int anio);
    Task<bool> EsFeriadoAsync(DateTime fecha);
}
