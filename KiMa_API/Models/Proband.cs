using System.ComponentModel.DataAnnotations;

namespace KiMa_API.Models
{
    public class Proband
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; }

        [Required, MaxLength(100)]
        public string LastName { get; set; }

        [Required, MaxLength(200)]
        public string Email { get; set; }

        public string Phone { get; set; }
        public int Age { get; set; }

        [Required]
        public int UserId { get; set; } // Verbindung zur User-Tabelle
    }
}

