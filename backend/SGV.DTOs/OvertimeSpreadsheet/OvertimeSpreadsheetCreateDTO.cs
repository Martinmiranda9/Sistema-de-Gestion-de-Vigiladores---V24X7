using System.ComponentModel.DataAnnotations;

namespace SGV.DTOs.OvertimeSpreadsheet;

public class OvertimeSpreadsheetCreateDTO
{
    [Required]
    public int WorkplaceId { get; set; }

    [Range(1, 12)]
    public int Month { get; set; }

    [Range(2000, 2100)]
    public int Year { get; set; }

    [Range(0, double.MaxValue)]
    public decimal ExtraHourRate { get; set; }

    public DateTime? RateValidFrom { get; set; }

    [MinLength(1)]
    public List<OvertimeSpreadsheetRowCreateDTO> Rows { get; set; } = [];
}
