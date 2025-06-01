using FluentValidation;
using Core.Entities;

namespace Core.Validators;

public class CarValidator : AbstractValidator<Car>
{
    public CarValidator()
    {
        RuleFor(c => c.VIN)
            .NotEmpty().WithMessage("VIN cant be empty.")
            .Length(17).WithMessage("VIN has to be 17 chars long.")
            .Matches("^[A-HJ-NPR-Z0-9]{17}$").WithMessage("Invalid format for VIN.");

        RuleFor(c => c.Mark)
            .NotEmpty().WithMessage("Mark of the Car can't be empty");

        RuleFor(c => c.Model)
            .NotEmpty().WithMessage("Model of the Car can't be empty");

        RuleFor(c => c.ProductionYear)
            .InclusiveBetween(1900, DateTime.Now.Year + 1).WithMessage("Year of production has to be inclusive between 1900 and 2026.");
    }
}