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

    [Column(TypeName = "decimal(18,2)")]
    public decimal ExtraHourRate { get; set; }

    public DateTime ValidFrom { get; set; }

    /// <summary>Motivo del cambio (ej: "Acuerdo paritario Abril 2026")</summary>
    [MaxLength(500)]
    public string? Reason { get; set; }

    /// <summary>Usuario que registró el cambio</summary>
    [MaxLength(100)]
    public string? ChangedBy { get; set; }

    /// <summary>Fecha en que se registró el cambio en el sistema</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
