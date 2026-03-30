using System.ComponentModel.DataAnnotations;

namespace SGV.DTOs.RegistroTurno;

public class RegistroTurnoCreateDTO
{
    [Required(ErrorMessage = "El vigilador es obligatorio.")]
    public int VigiladorId { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria.")]
    public DateTime Fecha { get; set; }

    [Required(ErrorMessage = "La hora de entrada es obligatoria.")]
    public DateTime HoraEntrada { get; set; }

    [Required(ErrorMessage = "La hora de salida es obligatoria.")]
    public DateTime HoraSalida { get; set; }

    [StringLength(500)]
    public string? Observaciones { get; set; }
}
