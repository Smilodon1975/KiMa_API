using KiMa_API.Models;
using KiMa_API.Models.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace KiMa_API.Services
{
    
    /// Service für die Authentifizierung, Benutzerregistrierung und Passwortverwaltung.    
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        private readonly JwtService _jwtService;
        private readonly IMailService _mailService;
        private readonly ILogger<AuthService> _logger;

        
        /// Konstruktor zur Initialisierung des AuthService mit UserManager, JWT-Service, MailService und Logger.
        
        public AuthService(UserManager<User> userManager, IConfiguration config, JwtService jwtService, IMailService mailService, ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _config = config;
            _jwtService = jwtService;
            _mailService = mailService;
            _logger = logger;
        }

       
        /// Authentifiziert einen Benutzer anhand von E-Mail und Passwort und gibt ein JWT-Token zurück.       
        /// <returns>Ein JWT-Token oder null, falls die Authentifizierung fehlschlägt.</returns>
        public async Task<string?> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
                return null;

            return _jwtService.GenerateJwtToken(user);
        }

        
        /// Registriert einen neuen Benutzer und speichert ihn in der Datenbank.        
        /// <returns>Ein IdentityResult-Objekt, das den Erfolg oder Fehler der Registrierung enthält.</returns>
        public async Task<IdentityResult> RegisterAsync(RegisterModel model)
        {
            _logger.LogInformation($"DEBUG: Eingehender UserName = '{model.UserName}'");

            var user = new User
            {
                Email = model.Email,
                UserName = string.IsNullOrWhiteSpace(model.UserName) ? model.Email : model.UserName,
                NormalizedUserName = model.UserName.ToUpper() // Manuelle Normalisierung
            };

            _logger.LogInformation($"DEBUG: Gespeicherter UserName = '{user.UserName}'");
            _logger.LogInformation($"DEBUG: Gespeicherter NormalizedUserName = '{user.NormalizedUserName}'");

            var result = await _userManager.CreateAsync(user, model.Password);
            return result;
        }

        
        /// Erstellt einen Passwort-Reset-Token und sendet es per E-Mail an den Benutzer.
        /// <returns>Das generierte Reset-Token oder null, falls der Benutzer nicht gefunden wurde.</returns>
        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return null;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _mailService.SendPasswordResetEmailAsync(email, token);

            return token;
        }

       
        /// Setzt das Passwort eines Benutzers anhand eines Reset-Tokens zurück.   
        /// <returns>True, wenn das Passwort erfolgreich zurückgesetzt wurde, andernfalls False.</returns>
        public async Task<bool> ResetPasswordAsync(PasswordResetDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return false;

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            return result.Succeeded;
        }
    }
}
