namespace SGV.DTOs.PayrollConfig;

public class PayrollConfigDTO
{
    public int Id { get; set; }
    public decimal NormalHourRate { get; set; }
    public decimal NightSurchargeRate { get; set; }
    public decimal HolidayHourRate { get; set; }
    public DateTime ValidFrom { get; set; }
}
