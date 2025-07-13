using Microsoft.Extensions.DependencyInjection;
using ManualMovementsManager.Application.Helpers;

namespace ManualMovementsManager.CrossCutting.DependencyInjection;

public static class ApplicationServiceCollectionExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IResponseBuilder, ResponseBuilder>();
        
        return services;
    }
} 