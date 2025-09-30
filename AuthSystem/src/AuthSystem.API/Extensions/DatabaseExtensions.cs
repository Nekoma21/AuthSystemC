using AuthSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthSystem.API.Extensions;

public static class DatabaseExtensions
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            logger.LogInformation("Starting database migration...");
            
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Applying {Count} pending migrations", pendingMigrations.Count());
                await context.Database.MigrateAsync();
                logger.LogInformation("Database migration completed successfully");
            }
            else
            {
                logger.LogInformation("Database is already up to date");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database");
            throw;
        }
    }
}
