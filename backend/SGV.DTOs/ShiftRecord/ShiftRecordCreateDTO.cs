using System.ComponentModel.DataAnnotations;

namespace SGV.DTOs.ShiftRecord;

public class ShiftRecordCreateDTO
{
    [Required(ErrorMessage = "El vigilador es obligatorio.")]
    public int SecurityGuardId { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria.")]
    public DateTime Date { get; set; }

    [Required(ErrorMessage = "La hora de entrada es obligatoria.")]
    public DateTime StartTime { get; set; }

    [Required(ErrorMessage = "La hora de salida es obligatoria.")]
    public DateTime EndTime { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}
