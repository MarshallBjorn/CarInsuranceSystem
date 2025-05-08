using Core.Entities;

namespace Infrastructure.Repositories;

public interface IUserRepository
{
    Task<bool> RegisterAsync(User user, string password1, string password2);
    Task<User?> LoginAsync(string email, string password);
    Task<bool> UserExistsAsync(string email);
    Task<User?> GetByEmailAsync(string email);
}