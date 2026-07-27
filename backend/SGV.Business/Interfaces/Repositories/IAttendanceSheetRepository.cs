using SGV.Entities;

namespace SGV.Business.Interfaces.Repositories;

public interface IAttendanceSheetRepository
{
    Task<IEnumerable<AttendanceSheet>> GetAllAsync(int? workplaceId, int? securityGuardId, int? month, int? year);
    Task<AttendanceSheet?> GetByIdAsync(int id);
    Task<AttendanceSheet?> GetExistingAsync(int securityGuardId, int month, int year);
    Task<AttendanceSheet> CreateAsync(AttendanceSheet sheet);
    Task<bool> DeleteAsync(int id);
}
