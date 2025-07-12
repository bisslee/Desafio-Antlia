using FluentValidation;
using ManualMovements.Application.Commands.Customers.AddCustomer;
using Microsoft.Extensions.DependencyInjection;

namespace ManualMovements.CrossCutting.DependencyInjection
{
    public static class FluentValidationServiceCollectionExtension
    {
        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            // Registra todos os validadores da assembly do application
            services
                .AddValidatorsFromAssemblyContaining<AddCustomerRequest>();

            return services;
        }
    }
}
