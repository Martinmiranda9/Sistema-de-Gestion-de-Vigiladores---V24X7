using System.ComponentModel.DataAnnotations;

namespace SGV.DTOs.Holiday;

public class HolidayCreateDTO
{
    [Required(ErrorMessage = "La fecha es obligatoria.")]
    public DateTime Date { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;

    public bool IsRecurring { get; set; } = false;
}
