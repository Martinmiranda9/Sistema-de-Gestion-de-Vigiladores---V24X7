namespace SGV.DTOs.RegistroTurno;

public class RegistroTurnoDTO
{
    public int Id { get; set; }
    public int VigiladorId { get; set; }
    public string VigiladorNombreCompleto { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public DateTime HoraEntrada { get; set; }
    public DateTime HoraSalida { get; set; }
    public string? Observaciones { get; set; }
    public double TotalHoras => (HoraSalida - HoraEntrada).TotalHours;
}
