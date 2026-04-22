using SGV.DTOs.OvertimeSpreadsheet;

namespace SGV.Business.Interfaces;

public interface IOvertimeSpreadsheetService
{
    Task<OvertimeSpreadsheetDTO> CreateAsync(OvertimeSpreadsheetCreateDTO dto);
    Task<OvertimeSpreadsheetDTO?> GetByIdAsync(int id);
    Task<IEnumerable<OvertimeSpreadsheetSummaryDTO>> GetByFilterAsync(int? month, int? year, string? search);
}
