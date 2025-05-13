public class RegisterRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime BirthDate { get; set; } = new DateTime()!;
    public string Password1 { get; set; } = null!;
    public string Password2 { get; set; } = null!;
}