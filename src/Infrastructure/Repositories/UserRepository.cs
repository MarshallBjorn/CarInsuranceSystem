using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public UserRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        var user = await GetByEmailAsync(email);
        if (user == null || string.IsNullOrEmpty(user.PasswordHash))
            return null;

        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash) ? user : null;
    }

    public async Task<bool> RegisterAsync(User user, string password1, string password2)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        if (await context.Users.AnyAsync(u => u.Email == user.Email))
            return false;

        if (password1 != password2)
            return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password1);
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users.FindAsync(id);
    }
}