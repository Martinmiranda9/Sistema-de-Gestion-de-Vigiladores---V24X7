using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGV.Entities;

[Table("OvertimeSpreadsheets")]
public class OvertimeSpreadsheet
{
    [Key]
    public int Id { get; set; }

    public int WorkplaceId { get; set; }

    [ForeignKey(nameof(WorkplaceId))]
    public Workplace Workplace { get; set; } = null!;

    [Range(1, 12)]
    public int Month { get; set; }

    [Range(2000, 2100)]
    public int Year { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ExtraHourRate { get; set; }

    [Column(TypeName = "date")]
    public DateTime? RateValidFrom { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalHours { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal GrandTotal { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<OvertimeSpreadsheetRow> Rows { get; set; } = new List<OvertimeSpreadsheetRow>();
}
