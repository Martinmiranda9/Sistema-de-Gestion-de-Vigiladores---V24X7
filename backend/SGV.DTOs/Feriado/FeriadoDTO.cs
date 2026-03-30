namespace SGV.DTOs.Feriado;

public class FeriadoDTO
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public bool EsRecurrente { get; set; }
}
