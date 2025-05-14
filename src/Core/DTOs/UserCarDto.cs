namespace Core.DTOs;

public class UserCarDto
{
    public Guid UserId { get; set; }
    public required string CarVIN { get; set; }
    public DateTime PurchaseDate { get; set; }
    public bool IsCurrentOwner { get; set; }
}
