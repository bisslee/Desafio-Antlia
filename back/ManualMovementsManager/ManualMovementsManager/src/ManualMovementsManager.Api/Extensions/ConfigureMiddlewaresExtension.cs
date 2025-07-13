using Biss.MultiSinkLogger.Extensions;
using Microsoft.Extensions.Options;
using ManualMovementsManager.Api.Middleware;

namespace ManualMovementsManager.Api.Extensions
{
    public static class ConfigureMiddlewaresExtension
    {
        public static WebApplication ConfigureMiddlewares(this WebApplication app)
        {
            var localizationOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>()?.Value;
            if (localizationOptions != null)
            {
                app.UseRequestLocalization(localizationOptions);
            }

            app.UseStructuredLogging();
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
            app.UseExceptionLogging();
            app.UseCustomLogging();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Configurar CORS baseado no ambiente
            app.ConfigureCorsPolicy();
            
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            return app;
        }

        private static void ConfigureCorsPolicy(this WebApplication app)
        {
            var environment = app.Environment.EnvironmentName;
            
            switch (environment.ToLower())
            {
                case "development":
                    app.UseCors("DevelopmentPolicy");
                    break;
                case "production":
                    app.UseCors("ProductionPolicy");
                    break;
                case "staging":
                    app.UseCors("ProductionPolicy"); // Usar política de produção para staging
                    break;
                default:
                    app.UseCors("DevelopmentPolicy"); // Fallback para desenvolvimento
                    break;
            }
        }
    }
}
