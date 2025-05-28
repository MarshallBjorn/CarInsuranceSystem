namespace Core.DTOs;

public class CreateUpdateInsuranceTypeDto
{
    public string Name { get; set; } = default!;
    public string PolicyDescription { get; set; } = default!;
    public string PolicyNumber { get; set; } = default!;
    public Guid FirmId { get; set; }
}
