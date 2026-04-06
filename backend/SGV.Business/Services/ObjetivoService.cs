using Microsoft.EntityFrameworkCore;
using SGV.Business.Interfaces;
using SGV.Data;
using SGV.DTOs.Objetivo;

namespace SGV.Business.Services;

public class ObjetivoService : IObjetivoService
{
    private readonly SGVDbContext _context;

    public ObjetivoService(SGVDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ObjetivoDTO>> GetAllAsync()
    {
        return await _context.Objetivos
            .Where(o => o.Activo)
            .Select(o => MapToDTO(o))
            .ToListAsync();
    }

    public async Task<ObjetivoDTO?> GetByIdAsync(int id)
    {
        var objetivo = await _context.Objetivos.FindAsync(id);
        return objetivo == null ? null : MapToDTO(objetivo);
    }

    public async Task<ObjetivoDTO> CreateAsync(ObjetivoCreateDTO dto)
    {
        var objetivo = new Entities.Objetivo
        {
            Nombre = dto.Nombre,
            Direccion = dto.Direccion,
            Activo = true
        };

        _context.Objetivos.Add(objetivo);
        await _context.SaveChangesAsync();

        return MapToDTO(objetivo);
    }

    public async Task<ObjetivoDTO?> UpdateAsync(int id, ObjetivoUpdateDTO dto)
    {
        var objetivo = await _context.Objetivos.FindAsync(id);
        if (objetivo == null) return null;

        objetivo.Nombre = dto.Nombre;
        objetivo.Direccion = dto.Direccion;
        objetivo.Activo = dto.Activo;

        await _context.SaveChangesAsync();

        return MapToDTO(objetivo);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var objetivo = await _context.Objetivos.FindAsync(id);
        if (objetivo == null) return false;

        // Baja lógica
        objetivo.Activo = false;
        await _context.SaveChangesAsync();

        return true;
    }

    private static ObjetivoDTO MapToDTO(Entities.Objetivo o)
    {
        return new ObjetivoDTO
        {
            Id = o.Id,
            Nombre = o.Nombre,
            Direccion = o.Direccion,
            Activo = o.Activo
        };
    }
}
