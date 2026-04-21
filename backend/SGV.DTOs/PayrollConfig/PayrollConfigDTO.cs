namespace SGV.DTOs.PayrollConfig;

public class PayrollConfigDTO
{
    public int Id { get; set; }
    public decimal NormalHourRate { get; set; }
    public decimal NightSurchargeRate { get; set; }
    public decimal HolidayHourRate { get; set; }
    public decimal ExtraHourRate { get; set; }
    public DateTime ValidFrom { get; set; }
    public string? Reason { get; set; }
    public string? ChangedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
