using System.ComponentModel.DataAnnotations;

namespace SGV.DTOs.Objetivo;

public class ObjetivoUpdateDTO
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección es obligatoria.")]
    [StringLength(200)]
    public string Direccion { get; set; } = string.Empty;

    [Required]
    public bool Activo { get; set; }
}
