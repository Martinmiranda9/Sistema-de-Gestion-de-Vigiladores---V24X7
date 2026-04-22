namespace SGV.DTOs.OvertimeSpreadsheet;

public class OvertimeSpreadsheetRowDTO
{
    public int Id { get; set; }
    public int? SecurityGuardId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Dni { get; set; } = string.Empty;
    public string FileNumber { get; set; } = string.Empty;
    public decimal Hours { get; set; }
    public decimal Total { get; set; }
    public bool Verified { get; set; }
}
