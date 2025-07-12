using FluentValidation;
using ManualMovements.Application.Validators;

namespace ManualMovements.Application.Commands.Customers.RemoveCustomer
{
    public class RemoveCustomerValidator : AbstractValidator<RemoveCustomerRequest>
    {
        public RemoveCustomerValidator()
        {
            RuleFor(x => x.Id).ValidateId();
        }
    }
}
