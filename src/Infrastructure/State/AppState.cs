using Core.Entities;

namespace Infrastructure.State;

public class AppState
{
    public User? LoggedInUser { get; set; }
}