using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using KiMa_API.Models;

namespace KiMa_API.Data
{
    /// <summary>
    /// Der AppDbContext verwaltet die Datenbankverbindung und definiert die Identitätskonfiguration für Benutzer und Rollen.
    /// </summary>
    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        /// <summary>
        /// Konstruktor für die Konfiguration der Datenbankoptionen.
        /// </summary>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<FAQ> FAQs { get; set; }

        /// <summary>
        /// Konfiguriert die Datenbankmodelle und setzt Standardwerte.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Optional: Vordefinierte Rollen in die Datenbank einfügen
            builder.Entity<IdentityRole<int>>().HasData(
                new IdentityRole<int> { Id = 1, Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole<int> { Id = 2, Name = "Proband", NormalizedName = "PROBAND" }
            );

            // Setzt den Standardwert für das Erstellungsdatum eines Benutzers
            builder.Entity<User>().Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
