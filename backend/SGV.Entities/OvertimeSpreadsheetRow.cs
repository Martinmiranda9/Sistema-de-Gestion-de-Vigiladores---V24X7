using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGV.Entities;

[Table("OvertimeSpreadsheetRows")]
public class OvertimeSpreadsheetRow
{
    [Key]
    public int Id { get; set; }

    public int OvertimeSpreadsheetId { get; set; }

    [ForeignKey(nameof(OvertimeSpreadsheetId))]
    public OvertimeSpreadsheet OvertimeSpreadsheet { get; set; } = null!;

    public int? SecurityGuardId { get; set; }

    [ForeignKey(nameof(SecurityGuardId))]
    public SecurityGuard? SecurityGuard { get; set; }

    [Required]
    [StringLength(250)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(20)]
    public string Dni { get; set; } = string.Empty;

    [StringLength(50)]
    public string FileNumber { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Hours { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }

    public bool Verified { get; set; }
}
