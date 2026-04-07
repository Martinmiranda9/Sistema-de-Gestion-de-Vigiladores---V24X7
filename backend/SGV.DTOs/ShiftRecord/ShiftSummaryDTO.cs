namespace SGV.DTOs.ShiftRecord;

public class ShiftSummaryDTO
{
    public int SecurityGuardId { get; set; }
    public string SecurityGuardFullName { get; set; } = string.Empty;
    public int Month { get; set; }
    public int Year { get; set; }
    public int TotalShifts { get; set; }
    public double TotalHours { get; set; }
    public double NightHours { get; set; }
    public double HolidayHours { get; set; }
    public double NormalHours { get; set; }
}
