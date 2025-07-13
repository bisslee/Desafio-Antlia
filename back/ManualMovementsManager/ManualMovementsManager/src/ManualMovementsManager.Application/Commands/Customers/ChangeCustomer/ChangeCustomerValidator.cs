using FluentValidation;
using ManualMovementsManager.Application.Validators;

namespace ManualMovementsManager.Application.Commands.Customers.ChangeCustomer
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
