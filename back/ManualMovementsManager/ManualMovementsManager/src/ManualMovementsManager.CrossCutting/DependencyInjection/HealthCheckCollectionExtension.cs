using ManualMovementsManager.CrossCutting.Health;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;

namespace ManualMovementsManager.CrossCutting.DependencyInjection
{
    public static class HealthCheckCollectionExtension
    {
        public static IServiceCollection AddHealthChecksInjection(this IServiceCollection services)
        {
            services.AddHealthChecks()
                // Health check da API
                .AddCheck<ApiHealthCheck>(
                    name: "api_health",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "api", "core" },
                    timeout: TimeSpan.FromSeconds(10))
                
                // Health check do banco de dados
                .AddCheck<DatabaseHealthCheck>(
                    name: "database_health",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "database", "storage" },
                    timeout: TimeSpan.FromSeconds(30))
                
                // Health check de dependências externas
                .AddCheck<ExternalDependenciesHealthCheck>(
                    name: "external_dependencies_health",
                    failureStatus: HealthStatus.Degraded,
                    tags: new[] { "external", "network" },
                    timeout: TimeSpan.FromSeconds(15));

            return services;
        }
    }
}
