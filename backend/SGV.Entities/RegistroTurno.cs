using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGV.Entities;

[Table("RegistroTurnos")]
public class RegistroTurno
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int VigiladorId { get; set; }

    [ForeignKey("VigiladorId")]
    public Vigilador Vigilador { get; set; } = null!;

    [Required]
    [Column(TypeName = "date")]
    public DateTime Fecha { get; set; }

    [Required]
    public DateTime HoraEntrada { get; set; }

    [Required]
    public DateTime HoraSalida { get; set; }

    [StringLength(500)]
    public string? Observaciones { get; set; }
}
