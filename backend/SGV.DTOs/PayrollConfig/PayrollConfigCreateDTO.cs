using System.ComponentModel.DataAnnotations;

namespace SGV.DTOs.PayrollConfig;

public class PayrollConfigCreateDTO
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "El valor de hora normal debe ser mayor a 0.")]
    public decimal NormalHourRate { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal NightSurchargeRate { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "El valor de hora en feriado debe ser mayor a 0.")]
    public decimal HolidayHourRate { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "El valor de hora extra debe ser mayor a 0.")]
    public decimal ExtraHourRate { get; set; }

    [Required(ErrorMessage = "La fecha de vigencia es obligatoria.")]
    public DateTime ValidFrom { get; set; }

    [MaxLength(500)]
    public string? Reason { get; set; }

    [MaxLength(100)]
    public string? ChangedBy { get; set; }
}
