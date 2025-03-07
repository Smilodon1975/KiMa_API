using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace KiMa_API.Data
{

    /// Factory für die Erstellung des `AppDbContext` zur Verwendung mit Migrations- und Design-Time-Tools.

    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        
        /// Erstellt eine neue Instanz des `AppDbContext` mit der Konfiguration aus `appsettings.json`.
      
        public AppDbContext CreateDbContext(string[] args)
        {
            // 🔹 Konfigurationsdatei laden
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            // 🔹 Verbindungszeichenfolge abrufen oder Fallback auf SQLite setzen
            var connectionString = configuration.GetConnectionString("SQLiteConnection")
                               ?? "Data Source=KiMaDB.sqlite";

            // 🔹 Optionen für den DbContext erstellen
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite(connectionString); // SQLite als Datenbank verwenden

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
