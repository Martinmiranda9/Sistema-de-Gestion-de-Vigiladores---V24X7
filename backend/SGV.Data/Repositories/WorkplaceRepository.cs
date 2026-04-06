using Microsoft.EntityFrameworkCore;
using SGV.Business.Interfaces.Repositories;
using SGV.Entities;

namespace SGV.Data.Repositories;

public class WorkplaceRepository : IWorkplaceRepository
{
    private readonly SGVDbContext _context;

    public WorkplaceRepository(SGVDbContext context) => _context = context;

    public async Task<IEnumerable<Workplace>> GetAllAsync(bool includeInactive = false)
        => await _context.Workplaces
            .Where(w => includeInactive || w.IsActive)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Workplace?> GetByIdAsync(int id)
        => await _context.Workplaces.FirstOrDefaultAsync(w => w.Id == id);

    public async Task AddAsync(Workplace workplace)
        => await _context.Workplaces.AddAsync(workplace);

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
