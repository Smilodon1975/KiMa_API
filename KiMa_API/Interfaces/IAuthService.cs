using KiMa_API.Models;
using KiMa_API.Models.Dto;
using Microsoft.AspNetCore.Identity;

namespace KiMa_API.Services
{

    /// Schnittstelle für Authentifizierungs- und Benutzerregistrierungsdienste.
    
    public interface IAuthService
    {
      
        /// Authentifiziert einen Benutzer anhand von E-Mail und Passwort.       
        /// <returns>Ein JWT-Token als Zeichenkette oder null, falls die Anmeldung fehlschlägt.</returns>
        Task<string?> LoginAsync(string email, string password);

       
        /// Registriert einen neuen Benutzer mit den angegebenen Daten.
        /// <returns>Ein `IdentityResult`, das den Erfolg oder Fehler der Registrierung enthält.</returns>
        Task<IdentityResult> RegisterAsync(RegisterModel model);

      
        /// Erstellt ein Token zum Zurücksetzen des Passworts für einen Benutzer.
        /// <returns>Ein Passwort-Reset-Token als Zeichenkette.</returns>
        Task<string> GeneratePasswordResetTokenAsync(string email);

    
        /// Setzt das Passwort eines Benutzers mithilfe eines Reset-Tokens zurück.
        /// <returns>True, wenn das Passwort erfolgreich zurückgesetzt wurde, andernfalls False.</returns>
        Task<bool> ResetPasswordAsync(PasswordResetDto model);
    }
}
