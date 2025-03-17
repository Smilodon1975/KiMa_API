using System.ComponentModel.DataAnnotations;

namespace KiMa_API.Models
{
    public class RegisterModel

    /// Modell für die Benutzerregistrierung mit E-Mail, Passwort und optionalem Benutzernamen.
    {
        [Required(ErrorMessage = "E-Mail ist erforderlich.")]
        [EmailAddress(ErrorMessage = "Ungültige E-Mail-Adresse.")]
        public required string Email { get; set; }




        [Required(ErrorMessage = "Passwort ist erforderlich.")]
        [MinLength(8, ErrorMessage = "Passwort muss mindestens 8 Zeichen lang sein.")]
        public required string Password { get; set; }


        
        public string? UserName { get; set; }
    }
}
