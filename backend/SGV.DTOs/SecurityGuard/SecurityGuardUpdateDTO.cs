using System.ComponentModel.DataAnnotations;

namespace SGV.DTOs.SecurityGuard;

public class SecurityGuardUpdateDTO
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [StringLength(20)]
    public string DNI { get; set; } = string.Empty;

    [StringLength(50)]
    public string FileNumber { get; set; } = string.Empty;

    public int? WorkplaceId { get; set; }

    public bool IsActive { get; set; }
}
