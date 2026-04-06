using Microsoft.EntityFrameworkCore;
using SGV.Business.Interfaces.Repositories;
using SGV.Entities;

namespace SGV.Data.Repositories;

public class HolidayRepository : IHolidayRepository
{
    private readonly SGVDbContext _context;

    public HolidayRepository(SGVDbContext context) => _context = context;

    public async Task<IEnumerable<Holiday>> GetAllAsync()
        => await _context.Holidays
            .OrderBy(h => h.Date)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Holiday?> GetByIdAsync(int id)
        => await _context.Holidays.FindAsync(id);

    public async Task<IEnumerable<Holiday>> GetByYearAsync(int year)
        => await _context.Holidays
            .Where(h => h.Date.Year == year || h.IsRecurring)
            .OrderBy(h => h.Date)
            .AsNoTracking()
            .ToListAsync();

    public async Task<bool> IsHolidayAsync(DateTime date)
        => await _context.Holidays.AnyAsync(h =>
            h.Date.Date == date.Date ||
            (h.IsRecurring && h.Date.Month == date.Month && h.Date.Day == date.Day));

    public async Task AddAsync(Holiday holiday)
        => await _context.Holidays.AddAsync(holiday);

    public void Remove(Holiday holiday)
        => _context.Holidays.Remove(holiday);

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
