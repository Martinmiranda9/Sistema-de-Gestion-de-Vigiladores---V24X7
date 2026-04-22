using SGV.Entities;

namespace SGV.Business.Interfaces.Repositories;

public interface IOvertimeSpreadsheetRepository
{
    Task AddAsync(OvertimeSpreadsheet spreadsheet);
    Task<OvertimeSpreadsheet?> GetByIdAsync(int id);
    Task<IEnumerable<OvertimeSpreadsheet>> GetByFilterAsync(int? month, int? year, string? search);
    Task SaveChangesAsync();
}
