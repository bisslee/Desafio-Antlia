using Biss.MultiSinkLogger;
using Microsoft.AspNetCore.HttpLogging;
using Serilog;
using Serilog.Context;

namespace ManualMovements.Api.Extensions
{
    public static class LoggingExtension
    {
        public static IServiceCollection ConfigureLogging(this IServiceCollection services, IConfiguration configuration)
        {
            // Configura o Biss.MultiSinkLogger
            LoggingManager.InitializeLogger(configuration);
            
            // Configura o Serilog no host
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(dispose: true);
            });

            // Configura HTTP logging para capturar requisições e respostas
            services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = HttpLoggingFields.All;
                logging.RequestHeaders.Add("Authorization");
                logging.RequestHeaders.Add("X-Correlation-ID");
                logging.ResponseHeaders.Add("X-Correlation-ID");
                logging.MediaTypeOptions.AddText("application/json");
                logging.RequestBodyLogLimit = 4096;
                logging.ResponseBodyLogLimit = 4096;
            });

            return services;
        }

        public static IApplicationBuilder UseStructuredLogging(this IApplicationBuilder app)
        {
            // Adiciona middleware para capturar correlation ID
            app.Use(async (context, next) =>
            {
                var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() 
                    ?? Guid.NewGuid().ToString();
                
                context.Response.Headers["X-Correlation-ID"] = correlationId;
                
                using (LogContext.PushProperty("CorrelationId", correlationId))
                using (LogContext.PushProperty("RequestPath", context.Request.Path))
                using (LogContext.PushProperty("RequestMethod", context.Request.Method))
                {
                    await next();
                }
            });

            // Adiciona middleware para capturar exceções
            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unhandled exception occurred during request processing");
                    throw;
                }
            });

            return app;
        }
    }
} 