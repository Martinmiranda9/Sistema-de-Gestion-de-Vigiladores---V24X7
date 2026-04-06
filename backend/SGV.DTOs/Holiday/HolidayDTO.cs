namespace SGV.DTOs.Holiday;

public class HolidayDTO
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsRecurring { get; set; }
}
