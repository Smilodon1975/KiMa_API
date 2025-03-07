using System.ComponentModel.DataAnnotations;

namespace KiMa_API.Models
{
    public class RegisterModel

    /// Modell für die Benutzerregistrierung mit E-Mail, Passwort und optionalem Benutzernamen.
    {
        [Required(ErrorMessage = "E-Mail ist erforderlich.")]
        [EmailAddress(ErrorMessage = "Ungültige E-Mail-Adresse.")]
        public string Email { get; set; }



        [Required(ErrorMessage = "Passwort ist erforderlich.")]
        [MinLength(6, ErrorMessage = "Passwort muss mindestens 6 Zeichen lang sein.")]
        public string Password { get; set; }


        
        public string? UserName { get; set; }
    }
}
