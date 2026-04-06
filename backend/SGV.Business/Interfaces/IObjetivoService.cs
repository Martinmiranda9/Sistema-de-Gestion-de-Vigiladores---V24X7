using SGV.DTOs.Objetivo;

namespace SGV.Business.Interfaces;

public interface IObjetivoService
{
    Task<IEnumerable<ObjetivoDTO>> GetAllAsync();
    Task<ObjetivoDTO?> GetByIdAsync(int id);
    Task<ObjetivoDTO> CreateAsync(ObjetivoCreateDTO dto);
    Task<ObjetivoDTO?> UpdateAsync(int id, ObjetivoUpdateDTO dto);
    Task<bool> DeleteAsync(int id);
}
