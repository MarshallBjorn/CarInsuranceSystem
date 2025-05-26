namespace Core.DTOs;

public class CreateCarDto
{
    public string VIN { get; set; } = default!;
    public string Mark { get; set; } = default!;
    public string Model { get; set; } = default!;
    public int ProductionYear { get; set; }
    public string? EngineType { get; set; }
    public List<CarInsuranceCreateDto> CarInsurances { get; set; } = new();
}