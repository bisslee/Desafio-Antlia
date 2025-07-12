using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace ManualMovementsManager.CrossCutting.DependencyInjection
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
