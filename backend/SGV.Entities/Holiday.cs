using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGV.Entities;

[Table("Holidays")]
public class Holiday
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "date")]
    public DateTime Date { get; set; }

    [Required]
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;

    public bool IsRecurring { get; set; } = false;
}
