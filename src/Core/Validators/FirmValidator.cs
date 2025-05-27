using Core.Entities;
using FluentValidation;

namespace App.Validators;

public class FirmValidator : AbstractValidator<Firm>
{
    public FirmValidator()
    {
        RuleFor(f => f.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(f => f.CountryCode)
            .NotEmpty().WithMessage("Country code is required.")
            .Length(2).WithMessage("Country code must be exactly 2 characters.")
            .Matches("^[A-Z]{2}$").WithMessage("Country code must be uppercase letters (e.g. 'US').");
    }
}
