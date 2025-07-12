using Microsoft.Extensions.DependencyInjection;
using System;

namespace ManualMovements.CrossCutting.DependencyInjection
{
    public static class MediatorServiceCollectionExtension
    {
        public static IServiceCollection AddMediator(this IServiceCollection services)
        {
            var assembly = AppDomain.CurrentDomain.Load("ManualMovements.Application");
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

            return services;
        }
    }
}
