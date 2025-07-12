using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using ManualMovementsManager.Infrastructure;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ManualMovementsManager.CrossCutting.Health
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DatabaseHealthCheck> _logger;
        private static readonly Stopwatch _stopwatch = new();

        public DatabaseHealthCheck(AppDbContext context, ILogger<DatabaseHealthCheck> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Executing Database Health Check");

            try
            {
                _stopwatch.Restart();

                // Verificar conectividade do banco
                var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
                if (!canConnect)
                {
                    _logger.LogError("Database connection failed");
                    return HealthCheckResult.Unhealthy("Database is not accessible");
                }

                // Verificar se há migrations pendentes
                var pendingMigrations = await _context.Database.GetPendingMigrationsAsync(cancellationToken);
                var hasPendingMigrations = pendingMigrations.Any();

                // Executar uma query simples para verificar performance
                var customerCount = await _context.Customers.CountAsync(cancellationToken);

                _stopwatch.Stop();

                var healthData = new Dictionary<string, object>
                {
                    { "ResponseTime", $"{_stopwatch.ElapsedMilliseconds}ms" },
                    { "CanConnect", canConnect },
                    { "HasPendingMigrations", hasPendingMigrations },
                    { "PendingMigrationsCount", pendingMigrations.Count() },
                    { "CustomerCount", customerCount },
                    { "DatabaseProvider", _context.Database.ProviderName ?? "Unknown" },
                    { "ConnectionString", GetConnectionStringInfo() }
                };

                var status = canConnect && !hasPendingMigrations ? HealthStatus.Healthy : HealthStatus.Degraded;
                var description = canConnect && !hasPendingMigrations 
                    ? "Database is healthy" 
                    : "Database has issues";

                _logger.LogInformation("Database Health Check completed in {ResponseTime}ms. Status: {Status}", 
                    _stopwatch.ElapsedMilliseconds, status);

                return new HealthCheckResult(status, description, data: healthData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database Health Check failed");
                return HealthCheckResult.Unhealthy("Database health check failed", ex);
            }
        }

        private static string GetConnectionStringInfo()
        {
            try
            {
                // Retorna apenas informações básicas da connection string para segurança
                return "Database connection configured";
            }
            catch
            {
                return "Connection string info unavailable";
            }
        }
    }
} 