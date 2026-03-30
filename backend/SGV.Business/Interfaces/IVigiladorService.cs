using SGV.DTOs.Vigilador;

namespace SGV.Business.Interfaces;

public interface IVigiladorService
{
    Task<IEnumerable<VigiladorDTO>> GetAllAsync();
    Task<VigiladorDTO?> GetByIdAsync(int id);
    Task<VigiladorDTO> CreateAsync(VigiladorCreateDTO dto);
    Task<VigiladorDTO?> UpdateAsync(int id, VigiladorUpdateDTO dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<VigiladorDTO>> GetByObjetivoAsync(string objetivo);
}
