using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGV.Entities;

[Table("AttendanceSheetRows")]
public class AttendanceSheetRow
{
    [Key]
    public int Id { get; set; }

    public int AttendanceSheetId { get; set; }

    [ForeignKey(nameof(AttendanceSheetId))]
    public AttendanceSheet AttendanceSheet { get; set; } = null!;

    public int Day { get; set; }

    [StringLength(10)]
    public string? Entry { get; set; }

    [StringLength(10)]
    public string? Exit { get; set; }

    public bool IsDayOff { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal WorkedHours { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal NightHours { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}
