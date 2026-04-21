using SGV.Business.Interfaces;
using SGV.Business.Interfaces.Repositories;
using SGV.DTOs.SecurityGuard;
using SGV.Entities;

namespace SGV.Business.Services;

public class SecurityGuardService : ISecurityGuardService
{
    private readonly ISecurityGuardRepository _repo;

    public SecurityGuardService(ISecurityGuardRepository repo) => _repo = repo;

    public async Task<IEnumerable<SecurityGuardDTO>> GetAllAsync()
    {
        var guards = await _repo.GetAllAsync(includeInactive: true);
        return guards.Select(MapToDTO);
    }

    public async Task<SecurityGuardDTO?> GetByIdAsync(int id)
    {
        var guard = await _repo.GetByIdAsync(id);
        return guard == null ? null : MapToDTO(guard);
    }

    public async Task<SecurityGuardDTO> CreateAsync(SecurityGuardCreateDTO dto)
    {
        var guard = new SecurityGuard
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            DNI = dto.DNI,
            FileNumber = dto.FileNumber,
            WorkplaceId = dto.WorkplaceId,
            IsActive = true
        };

        await _repo.AddAsync(guard);
        await _repo.SaveChangesAsync();

        var created = await _repo.GetByIdAsync(guard.Id);
        return MapToDTO(created!);
    }

    public async Task<SecurityGuardDTO?> UpdateAsync(int id, SecurityGuardUpdateDTO dto)
    {
        var guard = await _repo.GetByIdAsync(id);
        if (guard == null) return null;

        guard.FirstName = dto.FirstName;
        guard.LastName = dto.LastName;
        guard.DNI = dto.DNI;
        guard.FileNumber = dto.FileNumber;
        guard.WorkplaceId = dto.WorkplaceId;
        guard.IsActive = dto.IsActive;

        await _repo.SaveChangesAsync();
        return MapToDTO(guard);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var guard = await _repo.GetByIdAsync(id);
        if (guard == null) return false;

        _repo.Remove(guard);
        await _repo.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<SecurityGuardDTO>> GetByWorkplaceAsync(int workplaceId)
    {
        var guards = await _repo.GetByWorkplaceAsync(workplaceId);
        return guards.Select(MapToDTO);
    }

    private static SecurityGuardDTO MapToDTO(SecurityGuard g) => new()
    {
        Id = g.Id,
        FirstName = g.FirstName,
        LastName = g.LastName,
        DNI = g.DNI,
        FileNumber = g.FileNumber,
        WorkplaceId = g.WorkplaceId,
        WorkplaceName = g.Workplace?.Name,
        IsActive = g.IsActive
    };
}
