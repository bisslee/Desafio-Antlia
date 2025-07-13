using FluentValidation;

namespace ManualMovementsManager.Application.Queries.Products.GetProduct
{
    public class GetProductValidator : AbstractValidator<GetProductRequest>
    {
        public GetProductValidator()
        {
            // Sem validações customizadas
        }
    }
} 