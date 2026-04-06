using SGV.Entities;

namespace SGV.Business.Interfaces.Repositories;

public interface IPayrollConfigRepository
{
    Task<IEnumerable<PayrollConfig>> GetAllAsync();
    Task<PayrollConfig?> GetByIdAsync(int id);
    Task<PayrollConfig?> GetCurrentAsync(DateTime date);
    Task AddAsync(PayrollConfig config);
    void Remove(PayrollConfig config);
    Task SaveChangesAsync();
}
