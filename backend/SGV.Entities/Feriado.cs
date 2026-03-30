using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGV.Entities;

[Table("Feriados")]
public class Feriado
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "date")]
    public DateTime Fecha { get; set; }

    [Required]
    [StringLength(200)]
    public string Descripcion { get; set; } = string.Empty;

    [Required]
    public bool EsRecurrente { get; set; } = false;
}
