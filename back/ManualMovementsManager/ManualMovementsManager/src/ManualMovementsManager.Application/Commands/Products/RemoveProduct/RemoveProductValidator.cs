using FluentValidation;
using ManualMovementsManager.Application.Validators;

namespace ManualMovementsManager.Application.Commands.Products.RemoveProduct
{
    public class RemoveProductValidator : AbstractValidator<RemoveProductRequest>
    {
        public RemoveProductValidator()
        {
            RuleFor(x => x.Id).ValidateId();
        }
    }
} 