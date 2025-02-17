using System.ComponentModel.DataAnnotations;

namespace KiMa_API.Models
{
    public class RegisterModel
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6, ErrorMessage = "Passwort muss mindestens 6 Zeichen lang sein.")]
        public string Password { get; set; } 

        public string Role { get; set; } = "Proband"; // Standardmäßig Proband
    }
}

