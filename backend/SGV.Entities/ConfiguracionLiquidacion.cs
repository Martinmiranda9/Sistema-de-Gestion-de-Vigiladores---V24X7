using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGV.Entities;

[Table("ConfiguracionesLiquidacion")]
public class ConfiguracionLiquidacion
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorHoraNormal { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorHoraNocturnaAdicional { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorHoraFeriado { get; set; }

    [Required]
    public DateTime FechaDesde { get; set; }
}
