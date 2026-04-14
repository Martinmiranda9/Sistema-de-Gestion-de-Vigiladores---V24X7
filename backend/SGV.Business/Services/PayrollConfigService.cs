using SGV.Business.Interfaces;
using SGV.Business.Interfaces.Repositories;
using SGV.DTOs.PayrollConfig;
using SGV.Entities;

namespace SGV.Business.Services;

public class PayrollConfigService : IPayrollConfigService
{
    private readonly IPayrollConfigRepository _repo;

    public PayrollConfigService(IPayrollConfigRepository repo) => _repo = repo;

    public async Task<IEnumerable<PayrollConfigDTO>> GetAllAsync()
    {
        var configs = await _repo.GetAllAsync();
        return configs.Select(MapToDTO);
    }

    public async Task<PayrollConfigDTO?> GetByIdAsync(int id)
    {
        var config = await _repo.GetByIdAsync(id);
        return config == null ? null : MapToDTO(config);
    }

    public async Task<PayrollConfigDTO> CreateAsync(PayrollConfigCreateDTO dto)
    {
        var config = new PayrollConfig
        {
            NormalHourRate = dto.NormalHourRate,
            NightSurchargeRate = dto.NightSurchargeRate,
            HolidayHourRate = dto.HolidayHourRate,
            ExtraHourRate = dto.ExtraHourRate,
            ValidFrom = dto.ValidFrom.Date
        };

        await _repo.AddAsync(config);
        await _repo.SaveChangesAsync();
        return MapToDTO(config);
    }

    public async Task<PayrollConfigDTO?> UpdateAsync(int id, PayrollConfigUpdateDTO dto)
    {
        var config = await _repo.GetByIdAsync(id);
        if (config == null) return null;

        config.NormalHourRate = dto.NormalHourRate;
        config.NightSurchargeRate = dto.NightSurchargeRate;
        config.HolidayHourRate = dto.HolidayHourRate;
        config.ExtraHourRate = dto.ExtraHourRate;
        config.ValidFrom = dto.ValidFrom.Date;

        await _repo.SaveChangesAsync();
        return MapToDTO(config);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var config = await _repo.GetByIdAsync(id);
        if (config == null) return false;

        _repo.Remove(config);
        await _repo.SaveChangesAsync();
        return true;
    }

    public async Task<PayrollConfigDTO?> GetCurrentAsync(DateTime date)
    {
        var config = await _repo.GetCurrentAsync(date);
        return config == null ? null : MapToDTO(config);
    }

    private static PayrollConfigDTO MapToDTO(PayrollConfig c) => new()
    {
        Id = c.Id,
        NormalHourRate = c.NormalHourRate,
        NightSurchargeRate = c.NightSurchargeRate,
        HolidayHourRate = c.HolidayHourRate,
        ExtraHourRate = c.ExtraHourRate,
        ValidFrom = c.ValidFrom
    };
}
