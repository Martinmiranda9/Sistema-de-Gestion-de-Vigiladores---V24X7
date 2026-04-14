using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGV.Entities;

[Table("SecurityGuards")]
public class SecurityGuard
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string DNI { get; set; } = string.Empty;

    [StringLength(50)]
    public string FileNumber { get; set; } = string.Empty;

    public int? WorkplaceId { get; set; }

    [ForeignKey("WorkplaceId")]
    public Workplace? Workplace { get; set; }

    public bool IsActive { get; set; } = true;
}
