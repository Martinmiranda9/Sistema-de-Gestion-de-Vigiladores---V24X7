using Microsoft.EntityFrameworkCore;
using SGV.Business.Interfaces;
using SGV.Data;
using SGV.DTOs.RegistroTurno;

namespace SGV.Business.Services;

public class RegistroTurnoService : IRegistroTurnoService
{
    private readonly SGVDbContext _context;

    public RegistroTurnoService(SGVDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RegistroTurnoDTO>> GetAllAsync()
    {
        return await _context.RegistroTurnos
            .Include(rt => rt.Vigilador)
            .OrderByDescending(rt => rt.Fecha)
            .Select(rt => MapToDTO(rt))
            .ToListAsync();
    }

    public async Task<RegistroTurnoDTO?> GetByIdAsync(int id)
    {
        var registro = await _context.RegistroTurnos
            .Include(rt => rt.Vigilador)
            .FirstOrDefaultAsync(rt => rt.Id == id);

        return registro == null ? null : MapToDTO(registro);
    }

    public async Task<RegistroTurnoDTO> CreateAsync(RegistroTurnoCreateDTO dto)
    {
        var registro = new Entities.RegistroTurno
        {
            VigiladorId = dto.VigiladorId,
            Fecha = dto.Fecha.Date,
            HoraEntrada = dto.HoraEntrada,
            HoraSalida = dto.HoraSalida,
            Observaciones = dto.Observaciones
        };

        _context.RegistroTurnos.Add(registro);
        await _context.SaveChangesAsync();

        // Recargar con el Vigilador incluido
        await _context.Entry(registro).Reference(r => r.Vigilador).LoadAsync();

        return MapToDTO(registro);
    }

    public async Task<RegistroTurnoDTO?> UpdateAsync(int id, RegistroTurnoUpdateDTO dto)
    {
        var registro = await _context.RegistroTurnos
            .Include(rt => rt.Vigilador)
            .FirstOrDefaultAsync(rt => rt.Id == id);

        if (registro == null) return null;

        registro.VigiladorId = dto.VigiladorId;
        registro.Fecha = dto.Fecha.Date;
        registro.HoraEntrada = dto.HoraEntrada;
        registro.HoraSalida = dto.HoraSalida;
        registro.Observaciones = dto.Observaciones;

        await _context.SaveChangesAsync();

        return MapToDTO(registro);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var registro = await _context.RegistroTurnos.FindAsync(id);
        if (registro == null) return false;

        _context.RegistroTurnos.Remove(registro);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Obtiene los turnos de un vigilador en un mes/año específico (para el almanaque).
    /// </summary>
    public async Task<IEnumerable<RegistroTurnoDTO>> GetByVigiladorAsync(int vigiladorId, int mes, int anio)
    {
        return await _context.RegistroTurnos
            .Include(rt => rt.Vigilador)
            .Where(rt => rt.VigiladorId == vigiladorId
                      && rt.Fecha.Month == mes
                      && rt.Fecha.Year == anio)
            .OrderBy(rt => rt.Fecha)
            .Select(rt => MapToDTO(rt))
            .ToListAsync();
    }

    private static RegistroTurnoDTO MapToDTO(Entities.RegistroTurno rt)
    {
        return new RegistroTurnoDTO
        {
            Id = rt.Id,
            VigiladorId = rt.VigiladorId,
            VigiladorNombreCompleto = $"{rt.Vigilador.Apellido}, {rt.Vigilador.Nombre}",
            Fecha = rt.Fecha,
            HoraEntrada = rt.HoraEntrada,
            HoraSalida = rt.HoraSalida,
            Observaciones = rt.Observaciones
        };
    }
}
