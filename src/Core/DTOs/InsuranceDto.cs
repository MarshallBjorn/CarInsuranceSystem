namespace Core.DTOs;

public class InsuranceDto
{
    public Guid Id { get; set; }
    public string PolicyNumber { get; set; } = default!;
    public string Type { get; set; } = default!;
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public bool IsActive { get; set; }
}
