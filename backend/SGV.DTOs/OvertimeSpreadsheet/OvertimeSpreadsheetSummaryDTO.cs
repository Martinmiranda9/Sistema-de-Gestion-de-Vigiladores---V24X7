namespace SGV.DTOs.OvertimeSpreadsheet;

public class OvertimeSpreadsheetSummaryDTO
{
    public int Id { get; set; }
    public int WorkplaceId { get; set; }
    public string WorkplaceName { get; set; } = string.Empty;
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal ExtraHourRate { get; set; }
    public decimal TotalHours { get; set; }
    public decimal GrandTotal { get; set; }
    public int RowsCount { get; set; }
    public int VerifiedCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
