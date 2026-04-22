using SGV.Business.Interfaces;
using SGV.Business.Interfaces.Repositories;
using SGV.DTOs.OvertimeSpreadsheet;
using SGV.Entities;

namespace SGV.Business.Services;

public class OvertimeSpreadsheetService : IOvertimeSpreadsheetService
{
    private readonly IOvertimeSpreadsheetRepository _repo;
    private readonly IWorkplaceRepository _workplaceRepo;

    public OvertimeSpreadsheetService(
        IOvertimeSpreadsheetRepository repo,
        IWorkplaceRepository workplaceRepo)
    {
        _repo = repo;
        _workplaceRepo = workplaceRepo;
    }

    public async Task<OvertimeSpreadsheetDTO> CreateAsync(OvertimeSpreadsheetCreateDTO dto)
    {
        var workplace = await _workplaceRepo.GetByIdAsync(dto.WorkplaceId)
            ?? throw new InvalidOperationException("No se encontró el objetivo indicado.");

        var validRows = dto.Rows
            .Where(r => r.Hours > 0 || r.Total > 0)
            .ToList();

        if (validRows.Count == 0)
            throw new InvalidOperationException("La planilla debe contener al menos una fila con horas o monto.");

        var spreadsheet = new OvertimeSpreadsheet
        {
            WorkplaceId = dto.WorkplaceId,
            Month = dto.Month,
            Year = dto.Year,
            ExtraHourRate = dto.ExtraHourRate,
            RateValidFrom = dto.RateValidFrom?.Date,
            TotalHours = validRows.Sum(r => r.Hours),
            GrandTotal = validRows.Sum(r => r.Total),
            CreatedAt = DateTime.UtcNow,
            Rows = validRows.Select(r => new OvertimeSpreadsheetRow
            {
                SecurityGuardId = r.SecurityGuardId,
                FullName = r.FullName.Trim(),
                Dni = r.Dni?.Trim() ?? string.Empty,
                FileNumber = r.FileNumber?.Trim() ?? string.Empty,
                Hours = r.Hours,
                Total = r.Total,
                Verified = r.Verified
            }).ToList()
        };

        await _repo.AddAsync(spreadsheet);
        await _repo.SaveChangesAsync();

        spreadsheet.Workplace = workplace;
        return MapToDetailDto(spreadsheet);
    }

    public async Task<OvertimeSpreadsheetDTO?> GetByIdAsync(int id)
    {
        var spreadsheet = await _repo.GetByIdAsync(id);
        return spreadsheet == null ? null : MapToDetailDto(spreadsheet);
    }

    public async Task<IEnumerable<OvertimeSpreadsheetSummaryDTO>> GetByFilterAsync(int? month, int? year, string? search)
    {
        var spreadsheets = await _repo.GetByFilterAsync(month, year, search);
        return spreadsheets.Select(MapToSummaryDto);
    }

    private static OvertimeSpreadsheetSummaryDTO MapToSummaryDto(OvertimeSpreadsheet s) => new()
    {
        Id = s.Id,
        WorkplaceId = s.WorkplaceId,
        WorkplaceName = s.Workplace.Name,
        Month = s.Month,
        Year = s.Year,
        ExtraHourRate = s.ExtraHourRate,
        TotalHours = s.TotalHours,
        GrandTotal = s.GrandTotal,
        RowsCount = s.Rows.Count,
        VerifiedCount = s.Rows.Count(r => r.Verified),
        CreatedAt = s.CreatedAt
    };

    private static OvertimeSpreadsheetDTO MapToDetailDto(OvertimeSpreadsheet s) => new()
    {
        Id = s.Id,
        WorkplaceId = s.WorkplaceId,
        WorkplaceName = s.Workplace.Name,
        Month = s.Month,
        Year = s.Year,
        ExtraHourRate = s.ExtraHourRate,
        RateValidFrom = s.RateValidFrom,
        TotalHours = s.TotalHours,
        GrandTotal = s.GrandTotal,
        CreatedAt = s.CreatedAt,
        Rows = s.Rows
            .OrderBy(r => r.FullName)
            .Select(r => new OvertimeSpreadsheetRowDTO
            {
                Id = r.Id,
                SecurityGuardId = r.SecurityGuardId,
                FullName = r.FullName,
                Dni = r.Dni,
                FileNumber = r.FileNumber,
                Hours = r.Hours,
                Total = r.Total,
                Verified = r.Verified
            })
            .ToList()
    };
}
