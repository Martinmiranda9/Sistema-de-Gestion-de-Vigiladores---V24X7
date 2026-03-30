using Microsoft.EntityFrameworkCore;
using SGV.Business.Interfaces;
using SGV.Data;
using SGV.DTOs.Vigilador;

namespace SGV.Business.Services;

public class VigiladorService : IVigiladorService
{
    private readonly SGVDbContext _context;

    public VigiladorService(SGVDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<VigiladorDTO>> GetAllAsync()
    {
        return await _context.Vigiladores
            .Select(v => MapToDTO(v))
            .ToListAsync();
    }

    public async Task<VigiladorDTO?> GetByIdAsync(int id)
    {
        var vigilador = await _context.Vigiladores.FindAsync(id);
        return vigilador == null ? null : MapToDTO(vigilador);
    }

    public async Task<VigiladorDTO> CreateAsync(VigiladorCreateDTO dto)
    {
        var vigilador = new Entities.Vigilador
        {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            DNI = dto.DNI,
            Objetivo = dto.Objetivo,
            Activo = true
        };

        _context.Vigiladores.Add(vigilador);
        await _context.SaveChangesAsync();

        return MapToDTO(vigilador);
    }

    public async Task<VigiladorDTO?> UpdateAsync(int id, VigiladorUpdateDTO dto)
    {
        var vigilador = await _context.Vigiladores.FindAsync(id);
        if (vigilador == null) return null;

        vigilador.Nombre = dto.Nombre;
        vigilador.Apellido = dto.Apellido;
        vigilador.DNI = dto.DNI;
        vigilador.Objetivo = dto.Objetivo;
        vigilador.Activo = dto.Activo;

        await _context.SaveChangesAsync();

        return MapToDTO(vigilador);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var vigilador = await _context.Vigiladores.FindAsync(id);
        if (vigilador == null) return false;

        // Baja lógica
        vigilador.Activo = false;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<VigiladorDTO>> GetByObjetivoAsync(string objetivo)
    {
        return await _context.Vigiladores
            .Where(v => v.Objetivo.Contains(objetivo) && v.Activo)
            .Select(v => MapToDTO(v))
            .ToListAsync();
    }

    private static VigiladorDTO MapToDTO(Entities.Vigilador v)
    {
        return new VigiladorDTO
        {
            Id = v.Id,
            Nombre = v.Nombre,
            Apellido = v.Apellido,
            DNI = v.DNI,
            Objetivo = v.Objetivo,
            Activo = v.Activo
        };
    }
}
