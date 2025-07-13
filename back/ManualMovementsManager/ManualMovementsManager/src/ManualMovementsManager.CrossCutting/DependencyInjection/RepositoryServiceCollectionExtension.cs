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
            // Customer repositories
            services.AddScoped<IReadRepository<Customer>, ReadRepository<Customer>>();
            services.AddScoped<IWriteRepository<Customer>, WriteRepository<Customer>>();
            services.AddScoped<ICustomerReadRepository, CustomerReadRepository>();

            // Product repositories
            services.AddScoped<IReadRepository<Product>, ReadRepository<Product>>();
            services.AddScoped<IWriteRepository<Product>, WriteRepository<Product>>();
            services.AddScoped<IProductReadRepository, ProductReadRepository>();

            // ProductCosif repositories
            services.AddScoped<IReadRepository<ProductCosif>, ReadRepository<ProductCosif>>();
            services.AddScoped<IWriteRepository<ProductCosif>, WriteRepository<ProductCosif>>();
            services.AddScoped<IProductCosifReadRepository, ProductCosifReadRepository>();

            // ManualMovement repositories
            services.AddScoped<IReadRepository<ManualMovement>, ReadRepository<ManualMovement>>();
            services.AddScoped<IWriteRepository<ManualMovement>, WriteRepository<ManualMovement>>();
            services.AddScoped<IManualMovementReadRepository, ManualMovementReadRepository>();

            return services;
        }
    }
}
