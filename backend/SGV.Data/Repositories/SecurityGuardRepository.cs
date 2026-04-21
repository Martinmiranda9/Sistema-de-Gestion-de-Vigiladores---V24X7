using Microsoft.EntityFrameworkCore;
using SGV.Business.Interfaces.Repositories;
using SGV.Entities;

namespace SGV.Data.Repositories;

public class SecurityGuardRepository : ISecurityGuardRepository
{
    private readonly SGVDbContext _context;

    public SecurityGuardRepository(SGVDbContext context) => _context = context;

    public async Task<IEnumerable<SecurityGuard>> GetAllAsync(bool includeInactive = false)
        => await _context.SecurityGuards
            .Include(g => g.Workplace)
            .Where(g => includeInactive || g.IsActive)
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

    public async Task<bool> ExistsByDNIAsync(string dni, int? excludeId = null)
        => await _context.SecurityGuards
            .AnyAsync(g => g.DNI == dni && (excludeId == null || g.Id != excludeId));

    public async Task AddAsync(SecurityGuard securityGuard)
        => await _context.SecurityGuards.AddAsync(securityGuard);

    public void Remove(SecurityGuard securityGuard)
        => _context.SecurityGuards.Remove(securityGuard);

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
