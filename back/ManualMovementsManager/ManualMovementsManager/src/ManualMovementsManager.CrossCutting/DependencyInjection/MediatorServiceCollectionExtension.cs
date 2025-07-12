using Microsoft.Extensions.DependencyInjection;
using System;

namespace ManualMovementsManager.CrossCutting.DependencyInjection
{
    public static class MediatorServiceCollectionExtension
    {
        public static IServiceCollection AddMediator(this IServiceCollection services)
        {
            var assembly = AppDomain.CurrentDomain.Load("ManualMovementsManager.Application");
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

            return services;
        }
    }
}
