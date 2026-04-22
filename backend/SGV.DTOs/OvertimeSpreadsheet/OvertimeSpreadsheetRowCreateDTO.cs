using System.ComponentModel.DataAnnotations;

namespace SGV.DTOs.OvertimeSpreadsheet;

public class OvertimeSpreadsheetRowCreateDTO
{
    public int? SecurityGuardId { get; set; }

    [Required]
    [StringLength(250)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(20)]
    public string Dni { get; set; } = string.Empty;

    [StringLength(50)]
    public string FileNumber { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Hours { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Total { get; set; }

    public bool Verified { get; set; }
}
