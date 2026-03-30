namespace SGV.DTOs.ConfiguracionLiquidacion;

public class ConfiguracionLiquidacionDTO
{
    public int Id { get; set; }
    public decimal ValorHoraNormal { get; set; }
    public decimal ValorHoraNocturnaAdicional { get; set; }
    public decimal ValorHoraFeriado { get; set; }
    public DateTime FechaDesde { get; set; }
}
