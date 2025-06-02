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
    private readonly IValidator<string> _passwordValidator;

    public UserService(IUserRepository repository, IValidator<User> userValidator, IValidator<string> passwordValidator)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _userValidator = userValidator ?? throw new ArgumentNullException(nameof(userValidator));
        _passwordValidator = passwordValidator;
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

        if (await UserExistsAsync(user.Email))
            throw new EmailExistsException("Email is already used by another user.") { TriedEmail = user.Email };

        var userValidationResult = await _userValidator.ValidateAsync(user);
        if (!userValidationResult.IsValid)
            throw new ValidationException(userValidationResult.Errors);

        if (password1 != password2)
            throw new ArgumentException("Passwords do not match.");

        var passwordValidationResult = await _passwordValidator.ValidateAsync(password1);
        if (!passwordValidationResult.IsValid)
            throw new ValidationException(passwordValidationResult.Errors);

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
        var existingUser = await _repository.GetByIdAsync(updatedUser.Id);
        if (existingUser == null)
            throw new KeyNotFoundException("User not found.");

        // Check if email is being changed
        var isChangingEmail = !string.Equals(existingUser.Email, updatedUser.Email, StringComparison.OrdinalIgnoreCase);

        if (isChangingEmail)
        {
            var emailTaken = await _repository.UserExistsAsync(updatedUser.Email);
            if (emailTaken)
                throw new EmailExistsException("Email is already used by another user.") { TriedEmail = updatedUser.Email };
        }

        return await _repository.UpdateUserAsync(updatedUser);
    }

    public async Task<bool> ChangePasswordAsync(string email, string currentPassword, string newPassword, string confirmNewPassword)
    {
        if (newPassword != confirmNewPassword)
            throw new ArgumentException("New passwords do not match.");

        var passwordValidationResult = await _passwordValidator.ValidateAsync(newPassword);
        if (!passwordValidationResult.IsValid)
            throw new ValidationException(passwordValidationResult.Errors);

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