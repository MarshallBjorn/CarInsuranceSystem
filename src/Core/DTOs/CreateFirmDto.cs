namespace Core.DTOs;

public class CreateFirmDto
{
    public string Name { get; set; } = null!;
    public string CountryCode { get; set; } = null!;
    public Guid UserId { get; set; }
}

