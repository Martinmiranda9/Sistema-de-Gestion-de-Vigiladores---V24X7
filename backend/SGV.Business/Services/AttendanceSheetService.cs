using SGV.Business.Interfaces;
using SGV.Business.Interfaces.Repositories;
using SGV.DTOs.AttendanceSheet;
using SGV.Entities;

namespace SGV.Business.Services;

public class AttendanceSheetService : IAttendanceSheetService
{
    private readonly IAttendanceSheetRepository _repository;

    public AttendanceSheetService(IAttendanceSheetRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AttendanceSheetDTO>> GetAllAsync(int? workplaceId, int? securityGuardId, int? month, int? year)
    {
        var sheets = await _repository.GetAllAsync(workplaceId, securityGuardId, month, year);
        return sheets.Select(MapToDTO);
    }

    public async Task<AttendanceSheetDTO?> GetByIdAsync(int id)
    {
        var sheet = await _repository.GetByIdAsync(id);
        if (sheet == null) return null;

        var dto = MapToDTO(sheet);
        dto.Rows = sheet.Rows.Select(r => new AttendanceSheetRowDTO
        {
            Id = r.Id,
            Day = r.Day,
            Entry = r.Entry,
            Exit = r.Exit,
            IsDayOff = r.IsDayOff,
            WorkedHours = r.WorkedHours,
            NightHours = r.NightHours,
            Notes = r.Notes
        }).OrderBy(r => r.Day).ToList();

        return dto;
    }

    public async Task<AttendanceSheetDTO> CreateAsync(AttendanceSheetCreateDTO dto)
    {
        var existing = await _repository.GetExistingAsync(dto.SecurityGuardId, dto.Month, dto.Year);
        if (existing != null)
        {
            throw new InvalidOperationException($"Ya existe una planilla guardada para el vigilador en el período {dto.Month}/{dto.Year}.");
        }

        var sheet = new AttendanceSheet
        {
            SecurityGuardId = dto.SecurityGuardId,
            WorkplaceId = dto.WorkplaceId,
            Month = dto.Month,
            Year = dto.Year,
            TotalWorkedHours = dto.TotalWorkedHours,
            TotalNightHours = dto.TotalNightHours,
            TotalExtraHours = dto.TotalExtraHours,
            CreatedAt = DateTime.UtcNow,
            Rows = dto.Rows.Select(r => new AttendanceSheetRow
            {
                Day = r.Day,
                Entry = r.Entry,
                Exit = r.Exit,
                IsDayOff = r.IsDayOff,
                WorkedHours = r.WorkedHours,
                NightHours = r.NightHours,
                Notes = r.Notes
            }).ToList()
        };

        var created = await _repository.CreateAsync(sheet);
        
        // Return full DTO by fetching again
        var result = await GetByIdAsync(created.Id);
        return result ?? throw new Exception("Error al recuperar la planilla creada.");
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    private static AttendanceSheetDTO MapToDTO(AttendanceSheet s)
    {
        return new AttendanceSheetDTO
        {
            Id = s.Id,
            SecurityGuardId = s.SecurityGuardId,
            SecurityGuardName = s.SecurityGuard != null ? $"{s.SecurityGuard.LastName}, {s.SecurityGuard.FirstName}" : "",
            SecurityGuardDNI = s.SecurityGuard?.DNI ?? "",
            WorkplaceId = s.WorkplaceId,
            WorkplaceName = s.Workplace?.Name ?? "",
            Month = s.Month,
            Year = s.Year,
            TotalWorkedHours = s.TotalWorkedHours,
            TotalNightHours = s.TotalNightHours,
            TotalExtraHours = s.TotalExtraHours,
            CreatedAt = s.CreatedAt
        };
    }
}
