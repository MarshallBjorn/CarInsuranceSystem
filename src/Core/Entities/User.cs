namespace Core.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Email { get; set; }               // e.g., "user@example.com"
    public required string FirstName { get; set; }           // e.g., "John"
    public required string LastName { get; set; }            // e.g., "Doe"
    public DateTime BirthDate { get; set; }         // Age validation later

    public string? PasswordHash { get; set; }
    public string? Role { get; set; } = "User";

     public ICollection<UserCar> UserCars { get; set; } = new List<UserCar>();
}