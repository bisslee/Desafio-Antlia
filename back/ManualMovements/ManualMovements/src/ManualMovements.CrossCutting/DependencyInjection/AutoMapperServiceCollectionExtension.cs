using ManualMovements.Application.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace ManualMovements.CrossCutting.DependencyInjection
{
    public static class AutoMapperServiceCollectionExtension
    {
        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            // Use the correct overload of AddAutoMapper that accepts assemblies or types
            services.AddAutoMapper(cfg => cfg.AddProfile<MappingConfig>());

            return services;
        }
    }
}
