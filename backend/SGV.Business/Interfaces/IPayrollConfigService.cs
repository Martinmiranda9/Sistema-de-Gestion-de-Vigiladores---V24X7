using SGV.DTOs.PayrollConfig;

namespace SGV.Business.Interfaces;

public interface IPayrollConfigService
{
    Task<IEnumerable<PayrollConfigDTO>> GetAllAsync();
    Task<PayrollConfigDTO?> GetByIdAsync(int id);
    Task<PayrollConfigDTO> CreateAsync(PayrollConfigCreateDTO dto);
    Task<PayrollConfigDTO?> UpdateAsync(int id, PayrollConfigUpdateDTO dto);
    Task<bool> DeleteAsync(int id);
    Task<PayrollConfigDTO?> GetCurrentAsync(DateTime date);
}
