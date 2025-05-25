namespace Core.Entities;

public class CarInsurance
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string CarVIN { get; set; } = null!;
    public Car? Car { get; set; }

    public Guid InsuranceTypeId { get; set; }
    public InsuranceType? InsuranceType { get; set; }

    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public bool IsActive { get; set; } = true;
}