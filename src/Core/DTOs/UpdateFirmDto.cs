namespace Core.DTOs;

public class UpdateFirmDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string CountryCode { get; set; } = null!;
}
