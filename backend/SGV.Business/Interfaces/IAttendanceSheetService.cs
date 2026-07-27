using SGV.DTOs.AttendanceSheet;

namespace SGV.Business.Interfaces;

public interface IAttendanceSheetService
{
    Task<IEnumerable<AttendanceSheetDTO>> GetAllAsync(int? workplaceId, int? securityGuardId, int? month, int? year);
    Task<AttendanceSheetDTO?> GetByIdAsync(int id);
    Task<AttendanceSheetDTO> CreateAsync(AttendanceSheetCreateDTO dto);
    Task<bool> DeleteAsync(int id);
}
