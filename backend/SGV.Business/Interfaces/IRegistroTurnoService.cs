using SGV.DTOs.RegistroTurno;

namespace SGV.Business.Interfaces;

public interface IRegistroTurnoService
{
    Task<IEnumerable<RegistroTurnoDTO>> GetAllAsync();
    Task<RegistroTurnoDTO?> GetByIdAsync(int id);
    Task<RegistroTurnoDTO> CreateAsync(RegistroTurnoCreateDTO dto);
    Task<RegistroTurnoDTO?> UpdateAsync(int id, RegistroTurnoUpdateDTO dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<RegistroTurnoDTO>> GetByVigiladorAsync(int vigiladorId, int mes, int anio);
}
