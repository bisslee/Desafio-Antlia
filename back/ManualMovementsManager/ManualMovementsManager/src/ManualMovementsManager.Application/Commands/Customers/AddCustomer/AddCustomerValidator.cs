using FluentValidation;
using ManualMovementsManager.Application.Validators;

namespace ManualMovementsManager.Application.Commands.Customers.AddCustomer
{
    public class AddCustomerValidator : AbstractValidator<AddCustomerRequest>
    {
        public AddCustomerValidator()
        {
            RuleFor(x => x.FullName).ValidateName();

            RuleFor(x => x.Email).ValidateEmail();

            RuleFor(x => x.DocumentNumber).ValidateDocument();
        }
    }
}
