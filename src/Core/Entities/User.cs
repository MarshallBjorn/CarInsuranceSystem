namespace Core.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; }               // e.g., "user@example.com"
    public string FirstName { get; set; }           // e.g., "John"
    public string LastName { get; set; }            // e.g., "Doe"
    public DateTime BirthDate { get; set; }         // Age validation later

    // Navigation property (1 User → N UserCar links)
    public ICollection<UserCar> UserCars { get; set; } = new List<UserCar>();
}