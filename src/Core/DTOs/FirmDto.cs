namespace Core.DTOs;

public class FirmDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string CountryCode { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public List<InsuranceTypeDto>? InsuranceTypes { get; set; }
}
