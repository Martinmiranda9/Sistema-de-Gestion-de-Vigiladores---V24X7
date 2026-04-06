using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGV.Entities;

[Table("PayrollConfigs")]
public class PayrollConfig
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal NormalHourRate { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal NightSurchargeRate { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal HolidayHourRate { get; set; }

    [Required]
    public DateTime ValidFrom { get; set; }
}
