using SGV.Business.Interfaces;
using SGV.Business.Interfaces.Repositories;
using SGV.DTOs.Holiday;
using SGV.Entities;

namespace SGV.Business.Services;

public class HolidayService : IHolidayService
{
    private readonly IHolidayRepository _repo;

    public HolidayService(IHolidayRepository repo) => _repo = repo;

    public async Task<IEnumerable<HolidayDTO>> GetAllAsync()
    {
        var holidays = await _repo.GetAllAsync();
        return holidays.Select(MapToDTO);
    }

    public async Task<HolidayDTO?> GetByIdAsync(int id)
    {
        var holiday = await _repo.GetByIdAsync(id);
        return holiday == null ? null : MapToDTO(holiday);
    }

    public async Task<HolidayDTO> CreateAsync(HolidayCreateDTO dto)
    {
        var holiday = new Holiday
        {
            Date = dto.Date.Date,
            Description = dto.Description,
            IsRecurring = dto.IsRecurring
        };

        await _repo.AddAsync(holiday);
        await _repo.SaveChangesAsync();
        return MapToDTO(holiday);
    }

    public async Task<HolidayDTO?> UpdateAsync(int id, HolidayUpdateDTO dto)
    {
        var holiday = await _repo.GetByIdAsync(id);
        if (holiday == null) return null;

        holiday.Date = dto.Date.Date;
        holiday.Description = dto.Description;
        holiday.IsRecurring = dto.IsRecurring;

        await _repo.SaveChangesAsync();
        return MapToDTO(holiday);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var holiday = await _repo.GetByIdAsync(id);
        if (holiday == null) return false;

        _repo.Remove(holiday);
        await _repo.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<HolidayDTO>> GetByYearAsync(int year)
    {
        var holidays = await _repo.GetByYearAsync(year);
        return holidays.Select(MapToDTO);
    }

    public async Task<bool> IsHolidayAsync(DateTime date)
        => await _repo.IsHolidayAsync(date);

    private static HolidayDTO MapToDTO(Holiday h) => new()
    {
        Id = h.Id,
        Date = h.Date,
        Description = h.Description,
        IsRecurring = h.IsRecurring
    };
}
