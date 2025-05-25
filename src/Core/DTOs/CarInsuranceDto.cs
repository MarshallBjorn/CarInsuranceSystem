namespace Core.DTOs;

public class CarInsuranceDto
{
    public Guid Id { get; set; }
    public string CarVIN { get; set; } = string.Empty;
    public Guid InsuranceTypeId { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public bool IsActive { get; set; }
    public InsuranceTypeDto InsuranceType { get; set; } = null!;
}
