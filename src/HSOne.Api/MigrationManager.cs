using HSOne.Data;
using Microsoft.EntityFrameworkCore;

namespace HSOne.Api
{
    public static class MigrationManager
    {
        public static WebApplication MigrationDatabase(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    using (var context = services.GetRequiredService<HSOneContext>())
                    {
                        context.Database.Migrate();
                        var dataSeeder = new DataSeeder();
                        dataSeeder.SeedAsync(context).Wait();
                    };
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating or seeding the database.");
                }
            }
            return app;
        }
    } 
}
