namespace Core.DTOs;

public class FirmDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string CountryCode { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}
