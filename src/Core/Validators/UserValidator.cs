using FluentValidation;
using System.Text.RegularExpressions;
using Core.Entities;

namespace Core.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(user => user.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .Length(2, 50).WithMessage("First name must be between 2-50 characters")
            .Matches(@"^[a-zA-Z\s\-']+$").WithMessage("First name contains invalid characters");

        RuleFor(user => user.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .Length(2, 50).WithMessage("Last name must be between 2-50 characters")
            .Matches(@"^[a-zA-Z\s\-']+$").WithMessage("Last name contains invalid characters");

        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Valid email is required")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

        RuleFor(user => user.BirthDate)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateTime.Now.AddYears(-18)).WithMessage("Must be at least 18 years old")
            .GreaterThan(DateTime.Now.AddYears(-120)).WithMessage("Invalid date of birth");
    }
}
