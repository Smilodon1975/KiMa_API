using Microsoft.EntityFrameworkCore;
using KiMa_API.Models; // Hier kommen später die Datenbankmodelle rein

namespace KiMa_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Hier kommen später die Tabellen rein
        public DbSet<User> Users { get; set; }
        public DbSet<Proband> Probanden { get; set; }
    }
}

