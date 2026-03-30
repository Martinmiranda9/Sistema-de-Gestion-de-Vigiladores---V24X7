using System.ComponentModel.DataAnnotations;

namespace SGV.DTOs.ConfiguracionLiquidacion;

public class ConfiguracionLiquidacionCreateDTO
{
    [Required(ErrorMessage = "El valor de hora normal es obligatorio.")]
    public decimal ValorHoraNormal { get; set; }

    [Required(ErrorMessage = "El valor de hora nocturna adicional es obligatorio.")]
    public decimal ValorHoraNocturnaAdicional { get; set; }

    [Required(ErrorMessage = "El valor de hora feriado es obligatorio.")]
    public decimal ValorHoraFeriado { get; set; }

    [Required(ErrorMessage = "La fecha desde es obligatoria.")]
    public DateTime FechaDesde { get; set; }
}
