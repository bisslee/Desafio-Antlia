using Microsoft.Extensions.DependencyInjection;
using ManualMovements.Application.Helpers;

namespace ManualMovements.CrossCutting.DependencyInjection;

public static class ApplicationServiceCollectionExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IResponseBuilder, ResponseBuilder>();
        
        return services;
    }
} 