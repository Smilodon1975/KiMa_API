using System.ComponentModel.DataAnnotations;

namespace KiMa_API.Models
{

    /// Modell für die Benutzerauthentifizierung mit E-Mail und Passwort.

    public class LoginModel
    {



        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;


        [Required]
        public string Password { get; set; } = null!;
    }
}
