using FluentValidation;
using ManualMovementsManager.Application.Validators;

namespace ManualMovementsManager.Application.Queries.ManualMovements.GetManualMovementByKey
{
    public class GetManualMovementByKeyValidator : AbstractValidator<GetManualMovementByKeyRequest>
    {
        public GetManualMovementByKeyValidator()
        {
            RuleFor(x => x.Id).ValidateId();
        }
    }
} 