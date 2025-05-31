namespace Core.DTOs;

public class InsuranceTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string PolicyDescription { get; set; } = default!;
    public string PolicyNumber { get; set; } = default!;
    public decimal Price { get; set; }
    public Guid FirmId { get; set; }
    public FirmInsuranceDto FirmInsuranceDto { get; set; } = default!;
}

