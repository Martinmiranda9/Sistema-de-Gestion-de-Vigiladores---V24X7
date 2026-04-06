using Microsoft.EntityFrameworkCore;
using SGV.Business.Interfaces.Repositories;
using SGV.Entities;

namespace SGV.Data.Repositories;

public class SecurityGuardRepository : ISecurityGuardRepository
{
    private readonly SGVDbContext _context;

    public SecurityGuardRepository(SGVDbContext context) => _context = context;

    public async Task<IEnumerable<SecurityGuard>> GetAllAsync()
        => await _context.SecurityGuards
            .Include(g => g.Workplace)
            .AsNoTracking()
            .ToListAsync();

    public async Task<SecurityGuard?> GetByIdAsync(int id)
        => await _context.SecurityGuards
            .Include(g => g.Workplace)
            .FirstOrDefaultAsync(g => g.Id == id);

    public async Task<IEnumerable<SecurityGuard>> GetByWorkplaceAsync(int workplaceId)
        => await _context.SecurityGuards
            .Include(g => g.Workplace)
            .Where(g => g.WorkplaceId == workplaceId && g.IsActive)
            .AsNoTracking()
            .ToListAsync();

    public async Task<bool> ExistsByNationalIdAsync(string nationalId, int? excludeId = null)
        => await _context.SecurityGuards
            .AnyAsync(g => g.NationalId == nationalId && (excludeId == null || g.Id != excludeId));

    public async Task AddAsync(SecurityGuard securityGuard)
        => await _context.SecurityGuards.AddAsync(securityGuard);

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
