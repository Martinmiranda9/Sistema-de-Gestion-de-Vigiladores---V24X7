namespace SGV.DTOs.Workplace;

public class WorkplaceDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
