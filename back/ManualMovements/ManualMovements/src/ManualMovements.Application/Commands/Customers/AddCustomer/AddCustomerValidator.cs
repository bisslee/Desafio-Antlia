using FluentValidation;
using ManualMovements.Application.Validators;

namespace ManualMovements.Application.Commands.Customers.AddCustomer
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
