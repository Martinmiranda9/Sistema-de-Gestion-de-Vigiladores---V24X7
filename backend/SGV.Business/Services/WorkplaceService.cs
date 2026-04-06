using SGV.Business.Interfaces;
using SGV.Business.Interfaces.Repositories;
using SGV.DTOs.Workplace;
using SGV.Entities;

namespace SGV.Business.Services;

public class WorkplaceService : IWorkplaceService
{
    private readonly IWorkplaceRepository _repo;

    public WorkplaceService(IWorkplaceRepository repo) => _repo = repo;

    public async Task<IEnumerable<WorkplaceDTO>> GetAllAsync()
    {
        var workplaces = await _repo.GetAllAsync(includeInactive: false);
        return workplaces.Select(MapToDTO);
    }

    public async Task<WorkplaceDTO?> GetByIdAsync(int id)
    {
        var workplace = await _repo.GetByIdAsync(id);
        return workplace == null ? null : MapToDTO(workplace);
    }

    public async Task<WorkplaceDTO> CreateAsync(WorkplaceCreateDTO dto)
    {
        var workplace = new Workplace
        {
            Name = dto.Name,
            Address = dto.Address,
            IsActive = true
        };

        await _repo.AddAsync(workplace);
        await _repo.SaveChangesAsync();
        return MapToDTO(workplace);
    }

    public async Task<WorkplaceDTO?> UpdateAsync(int id, WorkplaceUpdateDTO dto)
    {
        var workplace = await _repo.GetByIdAsync(id);
        if (workplace == null) return null;

        workplace.Name = dto.Name;
        workplace.Address = dto.Address;
        workplace.IsActive = dto.IsActive;

        await _repo.SaveChangesAsync();
        return MapToDTO(workplace);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var workplace = await _repo.GetByIdAsync(id);
        if (workplace == null) return false;

        workplace.IsActive = false;
        await _repo.SaveChangesAsync();
        return true;
    }

    private static WorkplaceDTO MapToDTO(Workplace w) => new()
    {
        Id = w.Id,
        Name = w.Name,
        Address = w.Address,
        IsActive = w.IsActive
    };
}
