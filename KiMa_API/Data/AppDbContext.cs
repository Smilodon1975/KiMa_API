using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using KiMa_API.Models;

namespace KiMa_API.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
       

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 🔹 Rollen hinzufügen
            builder.Entity<IdentityRole<int>>().HasData(
                new IdentityRole<int> { Id = 1, Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole<int> { Id = 2, Name = "Proband", NormalizedName = "PROBAND" }
            );

            // 🔹 Admin-Benutzer hinzufügen
            var adminUser = new User
            {
                Id = 1,
                UserName = "MasterAdmin",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                Role = "Admin",
                FirstName = "Admin",
                LastName = "User",  // 🔹 Diese Zeilen wurden hinzugefügt
                EmailConfirmed = true
            };

            adminUser.PasswordHash = new PasswordHasher<User>().HashPassword(adminUser, "Admin123!");

            builder.Entity<User>().Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

        }

    }



}


