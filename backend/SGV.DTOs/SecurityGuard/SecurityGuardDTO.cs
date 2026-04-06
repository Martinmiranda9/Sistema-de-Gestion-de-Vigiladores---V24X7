namespace SGV.DTOs.SecurityGuard;

public class SecurityGuardDTO
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DNI { get; set; } = string.Empty;
    public int? WorkplaceId { get; set; }
    public string? WorkplaceName { get; set; }
    public bool IsActive { get; set; }
    public string FullName => $"{LastName}, {FirstName}";
}
