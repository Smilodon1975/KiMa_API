using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace KiMa_API.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // 1. Environment auslesen (Default = Production)
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                              ?? "Production";

            // 2. Konfiguration laden, inkl. appsettings.Development.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .Build();

            // 3. ConnectionString ziehen
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                                   ?? throw new InvalidOperationException(
                                        "Fehlt DefaultConnection in appsettings.");

            // 4. DbContext-Optionen aufbauen
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            if (environment.Equals("Development", StringComparison.OrdinalIgnoreCase))
            {
                // Development → SQLite
                optionsBuilder.UseSqlite(connectionString);
            }
            else
            {
                // Production (bzw. alles andere) → MySQL
                optionsBuilder.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString)
                );
            }

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
