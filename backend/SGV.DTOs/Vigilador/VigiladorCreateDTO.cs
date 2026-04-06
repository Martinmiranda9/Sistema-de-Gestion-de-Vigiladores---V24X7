using System.ComponentModel.DataAnnotations;

namespace SGV.DTOs.Vigilador;

/// <summary>
/// DTO para crear un nuevo Vigilador.
/// </summary>
public class VigiladorCreateDTO
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(100)]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [StringLength(20)]
    public string DNI { get; set; } = string.Empty;

    public int? ObjetivoId { get; set; }
}
