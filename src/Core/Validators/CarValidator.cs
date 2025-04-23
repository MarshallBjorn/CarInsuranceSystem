using FluentValidation;
using Core.Entities;

namespace Core.Validators;

public class CarValidator : AbstractValidator<Car>
{
    public CarValidator()
    {
        RuleFor(c => c.VIN)
            .NotEmpty()
            .Length(17)
            .Matches("^[A-HJ-NPR-Z0-9]{17}$");  // Standard VIN format

        RuleFor(c => c.ProductionYear)
            .InclusiveBetween(1900, DateTime.Now.Year + 1);
    }
}