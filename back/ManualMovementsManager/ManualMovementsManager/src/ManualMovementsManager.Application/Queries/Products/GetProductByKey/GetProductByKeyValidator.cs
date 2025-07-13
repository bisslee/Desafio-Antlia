using FluentValidation;
using ManualMovementsManager.Application.Validators;

namespace ManualMovementsManager.Application.Queries.Products.GetProductByKey
{
    public class GetProductByKeyValidator : AbstractValidator<GetProductByKeyRequest>
    {
        public GetProductByKeyValidator()
        {
            RuleFor(x => x.Id).ValidateId();
        }
    }
} 