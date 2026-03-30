using Microsoft.EntityFrameworkCore;
using SGV.Business.Interfaces;
using SGV.Data;
using SGV.DTOs.ConfiguracionLiquidacion;

namespace SGV.Business.Services;

public class ConfiguracionLiquidacionService : IConfiguracionLiquidacionService
{
    private readonly SGVDbContext _context;

    public ConfiguracionLiquidacionService(SGVDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ConfiguracionLiquidacionDTO>> GetAllAsync()
    {
        return await _context.ConfiguracionesLiquidacion
            .OrderByDescending(c => c.FechaDesde)
            .Select(c => MapToDTO(c))
            .ToListAsync();
    }

    public async Task<ConfiguracionLiquidacionDTO?> GetByIdAsync(int id)
    {
        var config = await _context.ConfiguracionesLiquidacion.FindAsync(id);
        return config == null ? null : MapToDTO(config);
    }

    public async Task<ConfiguracionLiquidacionDTO> CreateAsync(ConfiguracionLiquidacionCreateDTO dto)
    {
        var config = new Entities.ConfiguracionLiquidacion
        {
            ValorHoraNormal = dto.ValorHoraNormal,
            ValorHoraNocturnaAdicional = dto.ValorHoraNocturnaAdicional,
            ValorHoraFeriado = dto.ValorHoraFeriado,
            FechaDesde = dto.FechaDesde.Date
        };

        _context.ConfiguracionesLiquidacion.Add(config);
        await _context.SaveChangesAsync();

        return MapToDTO(config);
    }

    public async Task<ConfiguracionLiquidacionDTO?> UpdateAsync(int id, ConfiguracionLiquidacionUpdateDTO dto)
    {
        var config = await _context.ConfiguracionesLiquidacion.FindAsync(id);
        if (config == null) return null;

        config.ValorHoraNormal = dto.ValorHoraNormal;
        config.ValorHoraNocturnaAdicional = dto.ValorHoraNocturnaAdicional;
        config.ValorHoraFeriado = dto.ValorHoraFeriado;
        config.FechaDesde = dto.FechaDesde.Date;

        await _context.SaveChangesAsync();

        return MapToDTO(config);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var config = await _context.ConfiguracionesLiquidacion.FindAsync(id);
        if (config == null) return false;

        _context.ConfiguracionesLiquidacion.Remove(config);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Obtiene la configuración vigente para una fecha dada.
    /// Busca la más reciente cuya FechaDesde sea menor o igual a la fecha del turno.
    /// </summary>
    public async Task<ConfiguracionLiquidacionDTO?> GetVigenteAsync(DateTime fecha)
    {
        var config = await _context.ConfiguracionesLiquidacion
            .Where(c => c.FechaDesde <= fecha.Date)
            .OrderByDescending(c => c.FechaDesde)
            .FirstOrDefaultAsync();

        return config == null ? null : MapToDTO(config);
    }

    private static ConfiguracionLiquidacionDTO MapToDTO(Entities.ConfiguracionLiquidacion c)
    {
        return new ConfiguracionLiquidacionDTO
        {
            Id = c.Id,
            ValorHoraNormal = c.ValorHoraNormal,
            ValorHoraNocturnaAdicional = c.ValorHoraNocturnaAdicional,
            ValorHoraFeriado = c.ValorHoraFeriado,
            FechaDesde = c.FechaDesde
        };
    }
}
