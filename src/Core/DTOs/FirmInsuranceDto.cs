namespace Core.DTOs;

public class FirmInsuranceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string CountryCode { get; set; } = default!;
}