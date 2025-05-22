using KiMa_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KiMa_API.Data
{

    // Der AppDbContext verwaltet die Datenbankverbindung und definiert die Identitätskonfiguration für Benutzer und Rollen.
    // wird genutzt von EF um die Datenbank Tabellen zu erstellen
    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {

        // Konstruktor für die Konfiguration der Datenbankoptionen.

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {// sofort beim Öffnen auf WAL umschalten
            var conn = Database.GetDbConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "PRAGMA journal_mode=WAL;";
            cmd.ExecuteNonQuery();
        }

        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<NewsletterSubscriber> NewsletterSubscribers { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }




        // Konfiguriert die Datenbankmodelle und setzt Standardwerte.

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

            // Konfiguriere die One‑to‑One‑Beziehung zwischen User und UserProfile
            builder.Entity<User>()
                   .HasOne(u => u.UserProfile)           // User hat ein Profile
                   .WithOne(p => p.User)             // UserProfile gehört zu einem User
                   .HasForeignKey<UserProfile>(p => p.UserId); // Fremdschlüssel in UserProfile

            builder.Entity<User>()
                    .Property(u => u.CreatedAt)
                    .HasColumnType("datetime") // optional
                    .HasDefaultValueSql(Database.IsMySql()
                            ? "CURRENT_TIMESTAMP(6)"
                            : "CURRENT_TIMESTAMP");

        }
    }
}
