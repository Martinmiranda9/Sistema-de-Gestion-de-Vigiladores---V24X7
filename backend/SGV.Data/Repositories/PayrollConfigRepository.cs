using Microsoft.EntityFrameworkCore;
using SGV.Business.Interfaces.Repositories;
using SGV.Entities;

namespace SGV.Data.Repositories;

public class PayrollConfigRepository : IPayrollConfigRepository
{
    private readonly SGVDbContext _context;

    public PayrollConfigRepository(SGVDbContext context) => _context = context;

    public async Task<IEnumerable<PayrollConfig>> GetAllAsync()
        => await _context.PayrollConfigs
            .OrderByDescending(c => c.ValidFrom)
            .AsNoTracking()
            .ToListAsync();

    public async Task<PayrollConfig?> GetByIdAsync(int id)
        => await _context.PayrollConfigs.FindAsync(id);

    public async Task<PayrollConfig?> GetCurrentAsync(DateTime date)
        => await _context.PayrollConfigs
            .Where(c => c.ValidFrom <= date.Date)
            .OrderByDescending(c => c.ValidFrom)
            .FirstOrDefaultAsync();

    public async Task AddAsync(PayrollConfig config)
        => await _context.PayrollConfigs.AddAsync(config);

    public void Remove(PayrollConfig config)
        => _context.PayrollConfigs.Remove(config);

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
