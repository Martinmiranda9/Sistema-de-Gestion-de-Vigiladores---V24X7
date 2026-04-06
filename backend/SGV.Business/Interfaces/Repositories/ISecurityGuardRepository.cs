using SGV.Entities;

namespace SGV.Business.Interfaces.Repositories;

public interface ISecurityGuardRepository
{
    Task<IEnumerable<SecurityGuard>> GetAllAsync();
    Task<SecurityGuard?> GetByIdAsync(int id);
    Task<IEnumerable<SecurityGuard>> GetByWorkplaceAsync(int workplaceId);
    Task<bool> ExistsByNationalIdAsync(string nationalId, int? excludeId = null);
    Task AddAsync(SecurityGuard securityGuard);
    Task SaveChangesAsync();
}
