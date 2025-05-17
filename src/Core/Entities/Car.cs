using System.Text.Json.Serialization;

namespace Core.Entities;

public class Car
{
    public required string VIN { get; set; }                // Primary key (e.g., "1HGCM82633A123456")
    public required string Mark { get; set; }               // e.g., "Toyota"
    public required string Model { get; set; }              // e.g., "Corolla"
    public int ProductionYear { get; set; }        // e.g., 2020
    public string? EngineType { get; set; }         // e.g., "Hybrid"

    // Foreign key (1 Car → 1 Insurance, optional)
    public Guid? InsuranceId { get; set; }
    public Insurance? Insurance { get; set; }
}