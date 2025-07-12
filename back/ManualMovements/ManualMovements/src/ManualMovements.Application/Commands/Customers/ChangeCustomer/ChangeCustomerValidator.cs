using FluentValidation;
using ManualMovements.Application.Validators;

namespace ManualMovements.Application.Commands.Customers.ChangeCustomer
{
    public class ChangeCustomerValidator : AbstractValidator<ChangeCustomerRequest>
    {
        public ChangeCustomerValidator()
        {
            RuleFor(x => x.Id).ValidateId();
            RuleFor(x => x.FullName).ValidateName();
            RuleFor(x => x.Email).ValidateEmail();
            RuleFor(x => x.Phone).ValidatePhoneRequired();
        }
    }
}
