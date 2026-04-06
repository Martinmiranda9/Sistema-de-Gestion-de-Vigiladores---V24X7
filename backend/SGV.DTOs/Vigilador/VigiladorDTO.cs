namespace SGV.DTOs.Vigilador;

/// <summary>
/// DTO de lectura para Vigilador.
/// </summary>
public class VigiladorDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string DNI { get; set; } = string.Empty;
    public int? ObjetivoId { get; set; }
    public string? ObjetivoNombre { get; set; }
    public bool Activo { get; set; }
    public string NombreCompleto => $"{Apellido}, {Nombre}";
}
