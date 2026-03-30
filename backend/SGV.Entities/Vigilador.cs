using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGV.Entities;

[Table("Vigiladores")]
public class Vigilador
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Apellido { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string DNI { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Objetivo { get; set; } = string.Empty;

    [Required]
    public bool Activo { get; set; } = true;
}
