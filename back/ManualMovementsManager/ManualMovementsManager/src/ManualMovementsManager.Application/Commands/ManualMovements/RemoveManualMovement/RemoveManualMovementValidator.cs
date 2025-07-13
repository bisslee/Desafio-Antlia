using FluentValidation;
using ManualMovementsManager.Application.Validators;

namespace ManualMovementsManager.Application.Commands.ManualMovements.RemoveManualMovement
{
    public class RemoveManualMovementValidator : AbstractValidator<RemoveManualMovementRequest>
    {
        public RemoveManualMovementValidator()
        {
            RuleFor(x => x.Id).ValidateId();
        }
    }
} 