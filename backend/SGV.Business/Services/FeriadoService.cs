using Microsoft.EntityFrameworkCore;
using SGV.Business.Interfaces;
using SGV.Data;
using SGV.DTOs.Feriado;

namespace SGV.Business.Services;

public class FeriadoService : IFeriadoService
{
    private readonly SGVDbContext _context;

    public FeriadoService(SGVDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FeriadoDTO>> GetAllAsync()
    {
        return await _context.Feriados
            .OrderBy(f => f.Fecha)
            .Select(f => MapToDTO(f))
            .ToListAsync();
    }

    public async Task<FeriadoDTO?> GetByIdAsync(int id)
    {
        var feriado = await _context.Feriados.FindAsync(id);
        return feriado == null ? null : MapToDTO(feriado);
    }

    public async Task<FeriadoDTO> CreateAsync(FeriadoCreateDTO dto)
    {
        var feriado = new Entities.Feriado
        {
            Fecha = dto.Fecha.Date,
            Descripcion = dto.Descripcion,
            EsRecurrente = dto.EsRecurrente
        };

        _context.Feriados.Add(feriado);
        await _context.SaveChangesAsync();

        return MapToDTO(feriado);
    }

    public async Task<FeriadoDTO?> UpdateAsync(int id, FeriadoUpdateDTO dto)
    {
        var feriado = await _context.Feriados.FindAsync(id);
        if (feriado == null) return null;

        feriado.Fecha = dto.Fecha.Date;
        feriado.Descripcion = dto.Descripcion;
        feriado.EsRecurrente = dto.EsRecurrente;

        await _context.SaveChangesAsync();

        return MapToDTO(feriado);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var feriado = await _context.Feriados.FindAsync(id);
        if (feriado == null) return false;

        _context.Feriados.Remove(feriado);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<FeriadoDTO>> GetByAnioAsync(int anio)
    {
        return await _context.Feriados
            .Where(f => f.Fecha.Year == anio || f.EsRecurrente)
            .OrderBy(f => f.Fecha)
            .Select(f => MapToDTO(f))
            .ToListAsync();
    }

    /// <summary>
    /// Verifica si una fecha dada es feriado (comparando fecha exacta o recurrencia día/mes).
    /// </summary>
    public async Task<bool> EsFeriadoAsync(DateTime fecha)
    {
        return await _context.Feriados.AnyAsync(f =>
            f.Fecha.Date == fecha.Date ||
            (f.EsRecurrente && f.Fecha.Month == fecha.Month && f.Fecha.Day == fecha.Day)
        );
    }

    private static FeriadoDTO MapToDTO(Entities.Feriado f)
    {
        return new FeriadoDTO
        {
            Id = f.Id,
            Fecha = f.Fecha,
            Descripcion = f.Descripcion,
            EsRecurrente = f.EsRecurrente
        };
    }
}
