using SGV.Entities;

namespace SGV.Business.Interfaces.Repositories;

public interface ISecurityGuardRepository
{
    Task<IEnumerable<SecurityGuard>> GetAllAsync(bool includeInactive = false);
    Task<SecurityGuard?> GetByIdAsync(int id);
    Task<IEnumerable<SecurityGuard>> GetByWorkplaceAsync(int workplaceId);
    Task<bool> ExistsByDNIAsync(string dni, int? excludeId = null);
    Task AddAsync(SecurityGuard securityGuard);
    Task SaveChangesAsync();
}
