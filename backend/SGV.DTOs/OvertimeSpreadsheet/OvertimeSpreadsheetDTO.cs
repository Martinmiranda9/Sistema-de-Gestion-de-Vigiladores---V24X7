namespace SGV.DTOs.OvertimeSpreadsheet;

public class OvertimeSpreadsheetDTO
{
    public int Id { get; set; }
    public int WorkplaceId { get; set; }
    public string WorkplaceName { get; set; } = string.Empty;
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal ExtraHourRate { get; set; }
    public DateTime? RateValidFrom { get; set; }
    public decimal TotalHours { get; set; }
    public decimal GrandTotal { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OvertimeSpreadsheetRowDTO> Rows { get; set; } = [];
}
