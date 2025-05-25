namespace Core.DTOs;

public class InsuranceTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PolicyDescription { get; set; } = string.Empty;
    public string PolicyNumber { get; set; } = string.Empty;
    public Guid FirmId { get; set; }
}
