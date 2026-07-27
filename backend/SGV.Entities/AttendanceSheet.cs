using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGV.Entities;

[Table("AttendanceSheets")]
public class AttendanceSheet
{
    [Key]
    public int Id { get; set; }

    public int SecurityGuardId { get; set; }

    [ForeignKey(nameof(SecurityGuardId))]
    public SecurityGuard SecurityGuard { get; set; } = null!;

    public int WorkplaceId { get; set; }

    [ForeignKey(nameof(WorkplaceId))]
    public Workplace Workplace { get; set; } = null!;

    [Range(1, 12)]
    public int Month { get; set; }

    [Range(2000, 2100)]
    public int Year { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalWorkedHours { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalNightHours { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalExtraHours { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<AttendanceSheetRow> Rows { get; set; } = new List<AttendanceSheetRow>();
}
