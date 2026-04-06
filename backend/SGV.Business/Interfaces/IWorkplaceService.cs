using SGV.DTOs.Workplace;

namespace SGV.Business.Interfaces;

public interface IWorkplaceService
{
    Task<IEnumerable<WorkplaceDTO>> GetAllAsync();
    Task<WorkplaceDTO?> GetByIdAsync(int id);
    Task<WorkplaceDTO> CreateAsync(WorkplaceCreateDTO dto);
    Task<WorkplaceDTO?> UpdateAsync(int id, WorkplaceUpdateDTO dto);
    Task<bool> DeleteAsync(int id);
}
