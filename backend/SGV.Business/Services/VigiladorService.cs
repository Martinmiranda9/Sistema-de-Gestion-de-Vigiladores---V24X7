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
            .Include(v => v.Objetivo)
            .Select(v => MapToDTO(v))
            .ToListAsync();
    }

    public async Task<VigiladorDTO?> GetByIdAsync(int id)
    {
        var vigilador = await _context.Vigiladores
            .Include(v => v.Objetivo)
            .FirstOrDefaultAsync(v => v.Id == id);
        return vigilador == null ? null : MapToDTO(vigilador);
    }

    public async Task<VigiladorDTO> CreateAsync(VigiladorCreateDTO dto)
    {
        var vigilador = new Entities.Vigilador
        {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            DNI = dto.DNI,
            ObjetivoId = dto.ObjetivoId,
            Activo = true
        };

        _context.Vigiladores.Add(vigilador);
        await _context.SaveChangesAsync();
        
        // Cargar navigation property si es necesario
        if (vigilador.ObjetivoId.HasValue) {
            await _context.Entry(vigilador).Reference(v => v.Objetivo).LoadAsync();
        }

        return MapToDTO(vigilador);
    }

    public async Task<VigiladorDTO?> UpdateAsync(int id, VigiladorUpdateDTO dto)
    {
        var vigilador = await _context.Vigiladores
            .Include(v => v.Objetivo)
            .FirstOrDefaultAsync(v => v.Id == id);
            
        if (vigilador == null) return null;

        vigilador.Nombre = dto.Nombre;
        vigilador.Apellido = dto.Apellido;
        vigilador.DNI = dto.DNI;
        vigilador.ObjetivoId = dto.ObjetivoId;
        vigilador.Activo = dto.Activo;

        await _context.SaveChangesAsync();
        
        if (vigilador.ObjetivoId.HasValue && (vigilador.Objetivo == null || vigilador.Objetivo.Id != vigilador.ObjetivoId)) {
            await _context.Entry(vigilador).Reference(v => v.Objetivo).LoadAsync();
        } else if (!vigilador.ObjetivoId.HasValue) {
            vigilador.Objetivo = null;
        }

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

    public async Task<IEnumerable<VigiladorDTO>> GetByObjetivoAsync(int objetivoId)
    {
        return await _context.Vigiladores
            .Include(v => v.Objetivo)
            .Where(v => v.ObjetivoId == objetivoId && v.Activo)
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
            ObjetivoId = v.ObjetivoId,
            ObjetivoNombre = v.Objetivo?.Nombre,
            Activo = v.Activo
        };
    }
}
