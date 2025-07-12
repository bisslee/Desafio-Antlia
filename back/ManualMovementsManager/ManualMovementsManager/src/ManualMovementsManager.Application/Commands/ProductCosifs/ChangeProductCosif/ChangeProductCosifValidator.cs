using FluentValidation;
using ManualMovementsManager.Application.Validators;

namespace ManualMovementsManager.Application.Commands.ProductCosifs.ChangeProductCosif
{
    public class ChangeProductCosifValidator : AbstractValidator<ChangeProductCosifRequest>
    {
        public ChangeProductCosifValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Product COSIF ID is required");

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

            RuleFor(x => x.ClassificationCode)
                .NotEmpty()
                .WithMessage("Classification code is required")
                .Length(6)
                .WithMessage("Classification code must be exactly 6 characters");
        }
    }
} 