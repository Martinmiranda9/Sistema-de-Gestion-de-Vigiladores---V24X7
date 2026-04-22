using Microsoft.EntityFrameworkCore;
using SGV.Business.Interfaces.Repositories;
using SGV.Entities;

namespace SGV.Data.Repositories;

public class OvertimeSpreadsheetRepository : IOvertimeSpreadsheetRepository
{
    private readonly SGVDbContext _context;

    public OvertimeSpreadsheetRepository(SGVDbContext context) => _context = context;

    public async Task AddAsync(OvertimeSpreadsheet spreadsheet)
        => await _context.OvertimeSpreadsheets.AddAsync(spreadsheet);

    public async Task<OvertimeSpreadsheet?> GetByIdAsync(int id)
        => await _context.OvertimeSpreadsheets
            .Include(s => s.Workplace)
            .Include(s => s.Rows)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<IEnumerable<OvertimeSpreadsheet>> GetByFilterAsync(int? month, int? year, string? search)
    {
        var query = _context.OvertimeSpreadsheets
            .Include(s => s.Workplace)
            .Include(s => s.Rows)
            .AsQueryable();

        if (month.HasValue)
            query = query.Where(s => s.Month == month.Value);

        if (year.HasValue)
            query = query.Where(s => s.Year == year.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(s =>
                s.Workplace.Name.ToLower().Contains(term) ||
                s.Rows.Any(r => r.FullName.ToLower().Contains(term) || r.Dni.Contains(term)));
        }

        return await query
            .OrderByDescending(s => s.Year)
            .ThenByDescending(s => s.Month)
            .ThenByDescending(s => s.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
