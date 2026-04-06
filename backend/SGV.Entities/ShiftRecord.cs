using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGV.Entities;

[Table("ShiftRecords")]
public class ShiftRecord
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int SecurityGuardId { get; set; }

    public SecurityGuard SecurityGuard { get; set; } = null!;

    [Required]
    [Column(TypeName = "date")]
    public DateTime Date { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}
