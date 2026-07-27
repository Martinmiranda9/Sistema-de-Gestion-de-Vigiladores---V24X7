namespace SGV.DTOs.AttendanceSheet;

public class AttendanceSheetDTO
{
    public int Id { get; set; }
    public int SecurityGuardId { get; set; }
    public string SecurityGuardName { get; set; } = string.Empty;
    public string SecurityGuardDNI { get; set; } = string.Empty;
    public int WorkplaceId { get; set; }
    public string WorkplaceName { get; set; } = string.Empty;
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal TotalWorkedHours { get; set; }
    public decimal TotalNightHours { get; set; }
    public decimal TotalExtraHours { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<AttendanceSheetRowDTO> Rows { get; set; } = new();
}

public class AttendanceSheetRowDTO
{
    public int Id { get; set; }
    public int Day { get; set; }
    public string? Entry { get; set; }
    public string? Exit { get; set; }
    public bool IsDayOff { get; set; }
    public decimal WorkedHours { get; set; }
    public decimal NightHours { get; set; }
    public string? Notes { get; set; }
}

public class AttendanceSheetCreateDTO
{
    public int SecurityGuardId { get; set; }
    public int WorkplaceId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal TotalWorkedHours { get; set; }
    public decimal TotalNightHours { get; set; }
    public decimal TotalExtraHours { get; set; }
    public List<AttendanceSheetRowCreateDTO> Rows { get; set; } = new();
}

public class AttendanceSheetRowCreateDTO
{
    public int Day { get; set; }
    public string? Entry { get; set; }
    public string? Exit { get; set; }
    public bool IsDayOff { get; set; }
    public decimal WorkedHours { get; set; }
    public decimal NightHours { get; set; }
    public string? Notes { get; set; }
}
