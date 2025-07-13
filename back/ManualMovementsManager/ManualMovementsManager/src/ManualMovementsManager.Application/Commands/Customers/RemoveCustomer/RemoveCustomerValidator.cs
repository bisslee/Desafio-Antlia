using FluentValidation;
using ManualMovementsManager.Application.Validators;

namespace ManualMovementsManager.Application.Commands.Customers.RemoveCustomer
{
    public class RemoveCustomerValidator : AbstractValidator<RemoveCustomerRequest>
    {
        public RemoveCustomerValidator()
        {
            RuleFor(x => x.Id).ValidateId();
        }
    }
}
