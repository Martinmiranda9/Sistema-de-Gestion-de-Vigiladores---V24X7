using SGV.Entities;

namespace SGV.Business.Interfaces.Repositories;

public interface IWorkplaceRepository
{
    Task<IEnumerable<Workplace>> GetAllAsync(bool includeInactive = false);
    Task<Workplace?> GetByIdAsync(int id);
    Task AddAsync(Workplace workplace);
    Task SaveChangesAsync();
}
