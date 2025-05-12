namespace Core.Entities;

public class UserCar
{
    // Composite primary key (UserId + CarVIN)
    public Guid UserId { get; set; }
    public required string CarVIN { get; set; }

    public Car? Car { get; set; }
    public User? User { get; set; }

    // Additional fields
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
    public bool IsCurrentOwner { get; set; } = true;
}