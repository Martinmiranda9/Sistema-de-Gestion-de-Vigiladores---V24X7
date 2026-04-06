using SGV.Entities;

namespace SGV.Business.Interfaces.Repositories;

public interface IHolidayRepository
{
    Task<IEnumerable<Holiday>> GetAllAsync();
    Task<Holiday?> GetByIdAsync(int id);
    Task<IEnumerable<Holiday>> GetByYearAsync(int year);
    Task<bool> IsHolidayAsync(DateTime date);
    Task AddAsync(Holiday holiday);
    void Remove(Holiday holiday);
    Task SaveChangesAsync();
}
