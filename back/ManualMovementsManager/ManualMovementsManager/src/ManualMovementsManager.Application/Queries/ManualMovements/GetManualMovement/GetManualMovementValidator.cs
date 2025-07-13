using FluentValidation;

namespace ManualMovementsManager.Application.Queries.ManualMovements.GetManualMovement
{
    public class GetManualMovementValidator : AbstractValidator<GetManualMovementRequest>
    {
        public GetManualMovementValidator()
        {
            // Sem validações customizadas
        }
    }
} 