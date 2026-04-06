namespace SGV.DTOs.ShiftRecord;

public class ShiftRecordDTO
{
    public int Id { get; set; }
    public int SecurityGuardId { get; set; }
    public string SecurityGuardFullName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Notes { get; set; }
}
