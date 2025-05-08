using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    private readonly AppDbContext _context = context;

    public async Task<User?> LoginAsync(string email, string password)
    {
        if (password == "")
            throw new Exception("Email and password field cannot be empty.");
        var user = await GetByEmailAsync(email);
        if (user == null)
            throw new Exception("User with this email does not exist.");
        
        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash) ? user : throw new Exception("Wrong email or password.");
    }

    public async Task<bool> RegisterAsync(User user, string password1, string password2)
    {
        if (await UserExistsAsync(user.Email))
            return false;
        if (password1 != password2)
            throw new Exception("Passwords should match.");
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password1);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UserExistsAsync(string email) {
        if (email == "")
            throw new Exception("Email and password field cannot be empty.");
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<User?> GetByEmailAsync(string email) {
        if (email == "")
            throw new Exception("Email and password field cannot be empty.");
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
}