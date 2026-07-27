using Microsoft.EntityFrameworkCore;
using SGV.Business.Interfaces.Repositories;
using SGV.Entities;

namespace SGV.Data.Repositories;

public class AttendanceSheetRepository : IAttendanceSheetRepository
{
    private readonly SGVDbContext _context;

    public AttendanceSheetRepository(SGVDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AttendanceSheet>> GetAllAsync(int? workplaceId, int? securityGuardId, int? month, int? year)
    {
        var query = _context.AttendanceSheets
            .Include(s => s.SecurityGuard)
            .Include(s => s.Workplace)
            .AsQueryable();

        if (workplaceId.HasValue)
            query = query.Where(s => s.WorkplaceId == workplaceId.Value);

        if (securityGuardId.HasValue)
            query = query.Where(s => s.SecurityGuardId == securityGuardId.Value);

        if (month.HasValue)
            query = query.Where(s => s.Month == month.Value);

        if (year.HasValue)
            query = query.Where(s => s.Year == year.Value);

        return await query.OrderByDescending(s => s.Year)
                          .ThenByDescending(s => s.Month)
                          .ToListAsync();
    }

    public async Task<AttendanceSheet?> GetByIdAsync(int id)
    {
        return await _context.AttendanceSheets
            .Include(s => s.SecurityGuard)
            .Include(s => s.Workplace)
            .Include(s => s.Rows)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<AttendanceSheet?> GetExistingAsync(int securityGuardId, int month, int year)
    {
        return await _context.AttendanceSheets
            .FirstOrDefaultAsync(s => s.SecurityGuardId == securityGuardId && s.Month == month && s.Year == year);
    }

    public async Task<AttendanceSheet> CreateAsync(AttendanceSheet sheet)
    {
        _context.AttendanceSheets.Add(sheet);
        await _context.SaveChangesAsync();
        return sheet;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var sheet = await _context.AttendanceSheets.FindAsync(id);
        if (sheet == null) return false;

        _context.AttendanceSheets.Remove(sheet);
        await _context.SaveChangesAsync();
        return true;
    }
}
