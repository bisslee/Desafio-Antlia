using FluentValidation;
using ManualMovementsManager.Application.Validators;

namespace ManualMovementsManager.Application.Queries.ProductCosifs.GetProductCosifByKey
{
    public class GetProductCosifByKeyValidator : AbstractValidator<GetProductCosifByKeyRequest>
    {
        public GetProductCosifByKeyValidator()
        {
            RuleFor(x => x.Id).ValidateId();
        }
    }
} 