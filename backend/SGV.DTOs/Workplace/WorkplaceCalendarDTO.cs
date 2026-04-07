namespace SGV.DTOs.Workplace;

public class WorkplaceCalendarDTO
{
    public int WorkplaceId { get; set; }
    public string WorkplaceName { get; set; } = string.Empty;
    public int Month { get; set; }
    public int Year { get; set; }
    public List<CalendarDayDTO> Days { get; set; } = new();
}

public class CalendarDayDTO
{
    public DateTime Date { get; set; }
    public bool IsHoliday { get; set; }
    public List<CalendarShiftDTO> Shifts { get; set; } = new();
}

public class CalendarShiftDTO
{
    public int ShiftRecordId { get; set; }
    public int SecurityGuardId { get; set; }
    public string SecurityGuardFullName { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
