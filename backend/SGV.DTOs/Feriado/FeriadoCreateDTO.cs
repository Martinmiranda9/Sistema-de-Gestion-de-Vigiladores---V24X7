using System.ComponentModel.DataAnnotations;

namespace SGV.DTOs.Feriado;

public class FeriadoCreateDTO
{
    [Required(ErrorMessage = "La fecha es obligatoria.")]
    public DateTime Fecha { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(200)]
    public string Descripcion { get; set; } = string.Empty;

    [Required]
    public bool EsRecurrente { get; set; }
}
