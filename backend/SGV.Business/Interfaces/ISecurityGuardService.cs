using SGV.DTOs.SecurityGuard;

namespace SGV.Business.Interfaces;

public interface ISecurityGuardService
{
    Task<IEnumerable<SecurityGuardDTO>> GetAllAsync();
    Task<SecurityGuardDTO?> GetByIdAsync(int id);
    Task<SecurityGuardDTO> CreateAsync(SecurityGuardCreateDTO dto);
    Task<SecurityGuardDTO?> UpdateAsync(int id, SecurityGuardUpdateDTO dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<SecurityGuardDTO>> GetByWorkplaceAsync(int workplaceId);
}
