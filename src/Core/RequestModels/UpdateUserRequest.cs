namespace Core.RequestModels;

public class UpdateUserRequest
{
    public Guid Id { get; set; }
    public string Email { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public DateTime BirthDate { get; set; }
}
