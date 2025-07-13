using FluentValidation;
using ManualMovementsManager.Application.Commands;
using ManualMovementsManager.Application.Helpers;

namespace ManualMovementsManager.Application.Validators
{
    public class AddressValidator : AbstractValidator<AddressRequest>
    {
        public AddressValidator()
        {
            RuleFor(a => a.Street)
                .NotEmpty()
                .WithMessage(_ => ResourceHelper.GetResource(
                    "REQUIRED_STREET", "Rua é obrigatória."));

            RuleFor(a => a.Neighborhood)
                .NotEmpty()
                .WithMessage(_ => ResourceHelper.GetResource(
                    "REQUIRED_NEIGHBORHOOD", "Bairro é obrigatório."));

            RuleFor(a => a.City)
                .NotEmpty()
                .WithMessage(_ => ResourceHelper.GetResource(
                    "REQUIRED_CITY", "Cidade é obrigatória."));

            RuleFor(a => a.State)
                .NotEmpty()
                .WithMessage(_ => ResourceHelper.GetResource(
                    "REQUIRED_STATE", "Estado é obrigatório."));

            RuleFor(a => a.Country)
                .NotEmpty()
                .WithMessage(_ => ResourceHelper.GetResource(
                    "REQUIRED_COUNTRY", "País é obrigatório."));

            RuleFor(a => a.ZipCode)
                .NotEmpty()
                .WithMessage(_ => ResourceHelper.GetResource(
                    "REQUIRED_ZIPCODE", "CEP é obrigatório."));
        }
    }

}
