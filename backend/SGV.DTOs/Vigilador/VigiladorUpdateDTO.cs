using System.ComponentModel.DataAnnotations;

namespace SGV.DTOs.Vigilador;

/// <summary>
/// DTO para actualizar un Vigilador existente.
/// </summary>
public class VigiladorUpdateDTO
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

    [Required(ErrorMessage = "El objetivo es obligatorio.")]
    [StringLength(200)]
    public string Objetivo { get; set; } = string.Empty;

    [Required]
    public bool Activo { get; set; }
}
