using Core.Entities;
using Infrastructure.Repositories;
using FluentValidation;

namespace Infrastructure.Services;

public class EmailExistsException : Exception
{
    public EmailExistsException() : base() { }
    public EmailExistsException(string message) : base(message) { }
    public string TriedEmail { get; set; }
}

public class UserService
{
    private readonly IUserRepository _repository;
    private readonly IValidator<User> _userValidator;

    public UserService(IUserRepository repository, IValidator<User> userValidator)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _userValidator = userValidator ?? throw new ArgumentNullException(nameof(userValidator));
    }

    public async Task<User> GetByIdAsync(Guid id)
    {
        var user = await _repository.GetByIdAsync(id);
        return user ?? throw new KeyNotFoundException($"User with ID {id} not found.");
    }

    public async Task<User> LoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Email and password cannot be empty.");

        var user = await _repository.LoginAsync(email, password);
        return user ?? throw new InvalidOperationException("Invalid email or password.");
    }

    public async Task<bool> RegisterAsync(User user, string password1, string password2)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        if (await UserExistsAsync(user.Email)) throw new EmailExistsException("Email is already used by another user.") { TriedEmail = user.Email };

        var validationResult = await _userValidator.ValidateAsync(user);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        if (string.IsNullOrWhiteSpace(password1) || string.IsNullOrWhiteSpace(password2))
            throw new ArgumentException("Passwords cannot be empty.");

        if (password1 != password2)
            throw new ArgumentException("Passwords do not match.");

        return await _repository.RegisterAsync(user, password1, password2);
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.");

        return await _repository.UserExistsAsync(email);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.");

        return await _repository.GetByEmailAsync(email);
    }

    public async Task<bool> UpdateUserAsync(User updatedUser)
    {
        if (await UserExistsAsync(updatedUser.Email)) throw new EmailExistsException("Email is already used by another user.") { TriedEmail = updatedUser.Email };

        return await _repository.UpdateUserAsync(updatedUser);
    }

    public async Task<bool> ChangePasswordAsync(string email, string currentPassword, string newPassword, string confirmNewPassword)
    {
        if (newPassword != confirmNewPassword)
            throw new ArgumentException("New passwords do not match.");

        var user = await _repository.GetByEmailAsync(email);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            throw new ArgumentException("Current password is incorrect.");

        var newHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        var success = await _repository.UpdatePasswordAsync(email, newHash);

        if (!success)
            throw new InvalidOperationException("Failed to update password.");

        return true;
    }

    public async Task<int> CountAsync()
    {
        return await _repository.GetCountAsync();
    }
}