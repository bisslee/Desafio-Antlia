
using Biss.MultiSinkLogger;
using ManualMovements.Api.Extensions;
using Serilog;

namespace ManualMovements.Api
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configura o Logger (agora configurado na extensão)
            builder.Host.UseSerilog();

            // Configura os serviços
            builder.Services.ConfigureServices(builder.Configuration);

            var app = builder.Build();

            // Aplica migrations
            app.ApplyMigrations();

            // Configura Middlewares
            app.ConfigureMiddlewares();

            // Mapear endpoint de health check
            app.MapHealthChecks("/health");

            app.Run();
        }
    }

}
