using System.ComponentModel.DataAnnotations;

namespace KiMa_API.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; } // Passwort wird verschlüsselt gespeichert

        [Required]
        public string Role { get; set; } // "Admin" oder "Proband"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

