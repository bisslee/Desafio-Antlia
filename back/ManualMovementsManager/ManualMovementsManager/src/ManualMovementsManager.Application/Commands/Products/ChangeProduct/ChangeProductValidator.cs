using FluentValidation;
using ManualMovementsManager.Application.Validators;

namespace ManualMovementsManager.Application.Commands.Products.ChangeProduct
{
    public class ChangeProductValidator : AbstractValidator<ChangeProductRequest>
    {
        public ChangeProductValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Product ID is required");

            RuleFor(x => x.ProductCode)
                .NotEmpty()
                .WithMessage("Product code is required")
                .Length(4)
                .WithMessage("Product code must be exactly 4 characters");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required")
                .MaximumLength(30)
                .WithMessage("Description must not exceed 30 characters");
        }
    }
} 