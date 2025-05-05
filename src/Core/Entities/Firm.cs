// src/Core/Entities/Firm.cs
namespace Core.Entities;

public class Firm
{
    public Guid Id { get; set; } = Guid.NewGuid();  // Primary key
    public required string Name { get; set; }                // e.g., "Allianz"
    public required string CountryCode { get; set; }         // e.g., "DE"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property (1 Firm → N Insurances)
    public ICollection<Insurance> Insurances { get; set; } = new List<Insurance>();
}