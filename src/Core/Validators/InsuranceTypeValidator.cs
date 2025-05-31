namespace Core.Validators;

using FluentValidation;

public class InsuranceTypeValidator : AbstractValidator<InsuranceType>
{
    public InsuranceTypeValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Insurance name is required.")
            .Length(2, 50).WithMessage("Insurance name must be between 2 and 50 characters.");

        RuleFor(x => x.PolicyDescription)
            .NotEmpty().WithMessage("Policy description is required.")
            .Length(5, 500).WithMessage("Description must be between 5 and 500 characters.");

        RuleFor(x => x.Price)
            .NotEmpty().WithMessage("Price should not be empty.");

        RuleFor(x => x.PolicyNumber)
            .NotEmpty().WithMessage("Policy number is required.")
            .Matches(@"^POL-[A-Z]{2,5}-\d{3,10}$")
            .WithMessage("Policy number must be in format like 'POL-ABC-123'.");
    }
}