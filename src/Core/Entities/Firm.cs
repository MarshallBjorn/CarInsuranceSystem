// src/Core/Entities/Firm.cs
namespace Core.Entities;

public class Firm
{
    public Guid Id { get; set; } = Guid.NewGuid();  // Primary key
    public required string Name { get; set; }                // e.g., "Allianz"
    public required string CountryCode { get; set; }         // e.g., "DE"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid UserId { get; set; }

    public ICollection<InsuranceType> InsuranceTypes { get; set; } = new List<InsuranceType>();
}