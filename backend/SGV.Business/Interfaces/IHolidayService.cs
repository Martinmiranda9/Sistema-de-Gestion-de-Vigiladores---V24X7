using SGV.DTOs.Holiday;

namespace SGV.Business.Interfaces;

public interface IHolidayService
{
    Task<IEnumerable<HolidayDTO>> GetAllAsync();
    Task<HolidayDTO?> GetByIdAsync(int id);
    Task<HolidayDTO> CreateAsync(HolidayCreateDTO dto);
    Task<HolidayDTO?> UpdateAsync(int id, HolidayUpdateDTO dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<HolidayDTO>> GetByYearAsync(int year);
    Task<bool> IsHolidayAsync(DateTime date);
}
