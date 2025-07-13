using FluentValidation;

namespace ManualMovementsManager.Application.Queries.ProductCosifs.GetProductCosif
{
    public class GetProductCosifValidator : AbstractValidator<GetProductCosifRequest>
    {
        public GetProductCosifValidator()
        {
            // Sem validações customizadas
        }
    }
} 