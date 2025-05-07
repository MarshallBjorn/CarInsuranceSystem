using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    private readonly AppDbContext _context = context;

    public async Task<User?> LoginAsync(string email, string password)
    {
        var user = await GetByEmailAsync(email);
        if (user == null)
            throw new Exception("User with this email does not exist.");
        
        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash) ? user : throw new Exception("Wrong email or password.");
    }

    public async Task<bool> RegisterAsync(User user, string password)
    {
        if (await UserExistsAsync(user.Email))
            return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UserExistsAsync(string email) {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<User?> GetByEmailAsync(string email) {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
}