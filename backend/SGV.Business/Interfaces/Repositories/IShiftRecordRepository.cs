using SGV.Entities;

namespace SGV.Business.Interfaces.Repositories;

public interface IShiftRecordRepository
{
    Task<IEnumerable<ShiftRecord>> GetAllAsync();
    Task<ShiftRecord?> GetByIdAsync(int id);
    Task<IEnumerable<ShiftRecord>> GetBySecurityGuardAsync(int securityGuardId, int month, int year);
    Task AddAsync(ShiftRecord record);
    void Remove(ShiftRecord record);
    Task SaveChangesAsync();
}
