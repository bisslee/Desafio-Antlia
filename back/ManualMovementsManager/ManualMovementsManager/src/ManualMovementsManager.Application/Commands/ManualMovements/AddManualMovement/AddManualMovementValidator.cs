using FluentValidation;
using ManualMovementsManager.Application.Validators;
using System;

namespace ManualMovementsManager.Application.Commands.ManualMovements.AddManualMovement
{
    public class AddManualMovementValidator : AbstractValidator<AddManualMovementRequest>
    {
        public AddManualMovementValidator()
        {
            RuleFor(x => x.Month)
                .InclusiveBetween(1, 12)
                .WithMessage("Month must be between 1 and 12");

            RuleFor(x => x.Year)
                .InclusiveBetween(2020, 2030)
                .WithMessage("Year must be between 2020 and 2030");

            RuleFor(x => x.ProductCode)
                .NotEmpty()
                .WithMessage("Product code is required")
                .Length(4)
                .WithMessage("Product code must be exactly 4 characters");

            RuleFor(x => x.CosifCode)
                .NotEmpty()
                .WithMessage("COSIF code is required")
                .Length(11)
                .WithMessage("COSIF code must be exactly 11 characters");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required")
                .MaximumLength(50)
                .WithMessage("Description must not exceed 50 characters");

            RuleFor(x => x.MovementDate)
                .NotEmpty()
                .WithMessage("Movement date is required")
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("Movement date cannot be in the future");

            RuleFor(x => x.UserCode)
                .NotEmpty()
                .WithMessage("User code is required")
                .MaximumLength(10)
                .WithMessage("User code must not exceed 10 characters");

            RuleFor(x => x.Value)
                .GreaterThan(0)
                .WithMessage("Value must be greater than zero");
        }
    }
} 