using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using KiMa_API.Models;

namespace KiMa_API.Data
{
    
    /// Der AppDbContext verwaltet die Datenbankverbindung und definiert die Identitätskonfiguration für Benutzer und Rollen.
    
    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
       
        /// Konstruktor für die Konfiguration der Datenbankoptionen.
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

      
        /// Konfiguriert die Datenbankmodelle und setzt Standardwerte.
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 🔹 Vordefinierte Rollen in die Datenbank einfügen
            builder.Entity<IdentityRole<int>>().HasData(
                new IdentityRole<int> { Id = 1, Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole<int> { Id = 2, Name = "Proband", NormalizedName = "PROBAND" }
            );

            // 🔹 Standard-Admin-Benutzer erstellen
            var adminUser = new User
            {
                Id = 1,
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                Role = "Admin",
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true
            };

            // 🔹 Sichere Passwortverschlüsselung für den Admin-Benutzer
            adminUser.PasswordHash = new PasswordHasher<User>().HashPassword(adminUser, "Admin123!");

            // 🔹 Standardwert für das Erstellungsdatum eines Benutzers setzen
            builder.Entity<User>().Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
