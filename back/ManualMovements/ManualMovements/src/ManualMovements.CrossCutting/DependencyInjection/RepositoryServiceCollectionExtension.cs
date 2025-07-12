using ManualMovements.Domain.Entities;
using ManualMovements.Domain.Repositories;
using ManualMovements.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace ManualMovements.CrossCutting.DependencyInjection
{
    public static class RepositoryServiceCollectionExtension
    {
        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddScoped<IReadRepository<Customer>, ReadRepository<Customer>>();
            services.AddScoped<IWriteRepository<Customer>, WriteRepository<Customer>>();
            services.AddScoped<ICustomerReadRepository, CustomerReadRepository>();
            return services;
        }
    }
}
