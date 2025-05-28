using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Core.DTOs;
using Core.Entities;

public class InsuranceType
{
    public Guid Id { get; set; }
    public required string Name { get; set; } // "Basic", "Premium"
    public required string PolicyDescription { get; set; }
    public required string PolicyNumber { get; set; }

    public Guid FirmId { get; set; }
    public Firm? Firm { get; set; }

    public ICollection<CarInsurance> CarInsurances { get; set; } = new List<CarInsurance>();

    [NotMapped]
    [JsonPropertyName("firmDto")]
    public FirmDto FirmDto { get; set; } = null!;

    [NotMapped]
    [JsonPropertyName("firmInsuranceDto")]
    public FirmInsuranceDto FirmInsuranceDto { get; set; } = null!;
}
