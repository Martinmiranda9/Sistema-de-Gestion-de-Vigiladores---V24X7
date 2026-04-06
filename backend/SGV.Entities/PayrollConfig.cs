using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGV.Entities;

[Table("PayrollConfigs")]
public class PayrollConfig
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal NormalHourRate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal NightSurchargeRate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal HolidayHourRate { get; set; }

    public DateTime ValidFrom { get; set; }
}
