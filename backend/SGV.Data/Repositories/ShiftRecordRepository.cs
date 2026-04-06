using Microsoft.EntityFrameworkCore;
using SGV.Business.Interfaces.Repositories;
using SGV.Entities;

namespace SGV.Data.Repositories;

public class ShiftRecordRepository : IShiftRecordRepository
{
    private readonly SGVDbContext _context;

    public ShiftRecordRepository(SGVDbContext context) => _context = context;

    public async Task<IEnumerable<ShiftRecord>> GetAllAsync()
        => await _context.ShiftRecords
            .Include(r => r.SecurityGuard)
            .OrderByDescending(r => r.Date)
            .AsNoTracking()
            .ToListAsync();

    public async Task<ShiftRecord?> GetByIdAsync(int id)
        => await _context.ShiftRecords
            .Include(r => r.SecurityGuard)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<IEnumerable<ShiftRecord>> GetBySecurityGuardAsync(int securityGuardId, int month, int year)
        => await _context.ShiftRecords
            .Include(r => r.SecurityGuard)
            .Where(r => r.SecurityGuardId == securityGuardId
                     && r.Date.Month == month
                     && r.Date.Year == year)
            .OrderBy(r => r.Date)
            .AsNoTracking()
            .ToListAsync();

    public async Task AddAsync(ShiftRecord record)
        => await _context.ShiftRecords.AddAsync(record);

    public void Remove(ShiftRecord record)
        => _context.ShiftRecords.Remove(record);

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
