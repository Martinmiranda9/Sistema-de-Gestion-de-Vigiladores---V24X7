using System.ComponentModel.DataAnnotations;

namespace SGV.DTOs.Workplace;

public class WorkplaceUpdateDTO
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección es obligatoria.")]
    [StringLength(200)]
    public string Address { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}
