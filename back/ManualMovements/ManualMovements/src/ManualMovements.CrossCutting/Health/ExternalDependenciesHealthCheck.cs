using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ManualMovements.CrossCutting.Health
{
    public class ExternalDependenciesHealthCheck : IHealthCheck
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ExternalDependenciesHealthCheck> _logger;
        private static readonly Stopwatch _stopwatch = new();

        public ExternalDependenciesHealthCheck(
            IHttpClientFactory httpClientFactory,
            ILogger<ExternalDependenciesHealthCheck> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Executing External Dependencies Health Check");

            try
            {
                _stopwatch.Restart();

                var healthData = new Dictionary<string, object>();
                var overallStatus = HealthStatus.Healthy;
                var issues = new List<string>();

                // Verificar conectividade com internet (exemplo)
                var internetStatus = await CheckInternetConnectivity(cancellationToken);
                healthData["InternetConnectivity"] = internetStatus;
                if (internetStatus != "Connected")
                {
                    overallStatus = HealthStatus.Degraded;
                    issues.Add("Internet connectivity issues");
                }

                // Verificar DNS resolution
                var dnsStatus = await CheckDnsResolution(cancellationToken);
                healthData["DnsResolution"] = dnsStatus;
                if (dnsStatus != "Working")
                {
                    overallStatus = HealthStatus.Degraded;
                    issues.Add("DNS resolution issues");
                }

                // Verificar APIs externas (exemplo com uma API pública)
                var externalApiStatus = await CheckExternalApi(cancellationToken);
                healthData["ExternalApi"] = externalApiStatus;
                if (externalApiStatus != "Available")
                {
                    overallStatus = HealthStatus.Degraded;
                    issues.Add("External API issues");
                }

                _stopwatch.Stop();
                healthData["ResponseTime"] = $"{_stopwatch.ElapsedMilliseconds}ms";
                healthData["Timestamp"] = DateTime.UtcNow;

                var description = issues.Any() 
                    ? $"External dependencies have issues: {string.Join(", ", issues)}"
                    : "All external dependencies are healthy";

                _logger.LogInformation("External Dependencies Health Check completed in {ResponseTime}ms. Status: {Status}", 
                    _stopwatch.ElapsedMilliseconds, overallStatus);

                return new HealthCheckResult(overallStatus, description, data: healthData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "External Dependencies Health Check failed");
                return HealthCheckResult.Unhealthy("External dependencies health check failed", ex);
            }
        }

        private async Task<string> CheckInternetConnectivity(CancellationToken cancellationToken)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(5);
                
                var response = await client.GetAsync("https://httpbin.org/status/200", cancellationToken);
                return response.IsSuccessStatusCode ? "Connected" : "Unavailable";
            }
            catch
            {
                return "Disconnected";
            }
        }

        private async Task<string> CheckDnsResolution(CancellationToken cancellationToken)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(3);
                
                var response = await client.GetAsync("https://google.com", cancellationToken);
                return response.IsSuccessStatusCode ? "Working" : "Failing";
            }
            catch
            {
                return "Failing";
            }
        }

        private async Task<string> CheckExternalApi(CancellationToken cancellationToken)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(5);
                
                // Verificar uma API pública como exemplo
                var response = await client.GetAsync("https://jsonplaceholder.typicode.com/posts/1", cancellationToken);
                return response.IsSuccessStatusCode ? "Available" : "Unavailable";
            }
            catch
            {
                return "Unavailable";
            }
        }
    }
} 