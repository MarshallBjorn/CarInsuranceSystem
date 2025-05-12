// src/Core/Entities/Insurance.cs
namespace Core.Entities;

public class Insurance
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string PolicyNumber { get; set; }       // e.g., "POL-12345"
    public required string Type { get; set; }               // "Basic", "Premium"
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public bool IsActive { get; set; } = true;

    // Foreign key (1 Insurance → 1 Firm)
    public Guid FirmId { get; set; }
    public Firm? Firm { get; set; }
}