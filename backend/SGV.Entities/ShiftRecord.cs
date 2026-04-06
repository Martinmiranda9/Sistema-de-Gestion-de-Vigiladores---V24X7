using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGV.Entities;

[Table("ShiftRecords")]
public class ShiftRecord
{
    [Key]
    public int Id { get; set; }

    public int SecurityGuardId { get; set; }

    public SecurityGuard SecurityGuard { get; set; } = null!;

    [Column(TypeName = "date")]
    public DateTime Date { get; set; }

    [Column(TypeName = "time")]
    public TimeSpan StartTime { get; set; }

    [Column(TypeName = "time")]
    public TimeSpan EndTime { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}
