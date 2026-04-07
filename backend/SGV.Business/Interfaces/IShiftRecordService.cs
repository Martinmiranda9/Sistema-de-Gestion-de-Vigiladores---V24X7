using SGV.DTOs.ShiftRecord;
using SGV.DTOs.Workplace;

namespace SGV.Business.Interfaces;

public interface IShiftRecordService
{
    Task<IEnumerable<ShiftRecordDTO>> GetAllAsync();
    Task<ShiftRecordDTO?> GetByIdAsync(int id);
    Task<ShiftRecordDTO> CreateAsync(ShiftRecordCreateDTO dto);
    Task<ShiftRecordDTO?> UpdateAsync(int id, ShiftRecordUpdateDTO dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<ShiftRecordDTO>> GetBySecurityGuardAsync(int securityGuardId, int month, int year);
    Task<ShiftSummaryDTO?> GetSummaryAsync(int securityGuardId, int month, int year);
    Task<WorkplaceCalendarDTO?> GetWorkplaceCalendarAsync(int workplaceId, int month, int year);
}
