using FluentValidation;
using ManualMovementsManager.Application.Validators;

namespace ManualMovementsManager.Application.Commands.ProductCosifs.RemoveProductCosif
{
    public class RemoveProductCosifValidator : AbstractValidator<RemoveProductCosifRequest>
    {
        public RemoveProductCosifValidator()
        {
            RuleFor(x => x.Id).ValidateId();
        }
    }
} 