using ManualMovementsManager.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ManualMovementsManager.Api.Extensions
{
    public static class MigrationExtension
    {
        public static void ApplyMigrations(this IHost app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<AppDbContext>();
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<AppDbContext>>();
                logger.LogError(ex, "Ocorreu um erro ao aplicar as migrações");
            }
        }
    }
}
