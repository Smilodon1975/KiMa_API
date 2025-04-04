using KiMa_API.Models;
using KiMa_API.Models.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                Email = model.Email ?? throw new ArgumentNullException(nameof(model.Email)),
                UserName = string.IsNullOrWhiteSpace(model.UserName) ? model.Email : model.UserName,
                NormalizedUserName = (model.UserName ?? model.Email).ToUpper()
            };

            _logger.LogInformation($"DEBUG: Gespeicherter UserName = '{user.UserName}'");

            return await _userManager.CreateAsync(user, model.Password ?? throw new ArgumentNullException(nameof(model.Password)));
        }


        /// Erstellt einen Passwort-Reset-Token und sendet es per E-Mail an den Benutzer.
        /// <returns>Das generierte Reset-Token oder null, falls der Benutzer nicht gefunden wurde.</returns>
        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            _logger.LogInformation($"[DEBUG] Passwort-Reset-Token für E-Mail: {email}");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogError("[ERROR] Benutzer nicht gefunden!");
                return string.Empty;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            _logger.LogInformation($"[DEBUG] Generiertes Token: {token}");

            await _userManager.SetAuthenticationTokenAsync(user, "Default", "ResetPassword", token);

            var storedToken = await _userManager.GetAuthenticationTokenAsync(user, "Default", "ResetPassword");
            _logger.LogInformation($"[DEBUG] Token in DB gespeichert? {storedToken != null}");

            // Hier wird nun der Benutzername mitgegeben:
            await _mailService.SendPasswordResetEmailAsync(email, token, user.UserName);

            return token;
        }

        /// Setzt das Passwort eines Benutzers anhand eines Reset-Tokens zurück.   
        /// <returns>True, wenn das Passwort erfolgreich zurückgesetzt wurde, andernfalls False.</returns>
        public async Task<bool> ResetPasswordAsync(PasswordResetDto model)
        {
            _logger.LogInformation($"[DEBUG] Reset-Anfrage für E-Mail: {model.Email}");
            _logger.LogInformation($"[DEBUG] Erhaltenes Token: {model.Token}");

            if (model.Email is null || model.Token is null || model.NewPassword is null)
            {
                _logger.LogError("[ERROR] Fehlende Pflichtwerte in der Reset-Anfrage!");
                return false;
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogError("[ERROR] Benutzer nicht gefunden!");
                return false;
            }

            var isValid = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", model.Token);
            if (!isValid)
            {
                _logger.LogError("[ERROR] Token ist ungültig!");
                return false;
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded)
            {
                _logger.LogError("[ERROR] Fehler beim Zurücksetzen des Passworts:");
                foreach (var error in result.Errors)
                {
                    _logger.LogError($" - {error.Description}");
                }
                return false;
            }

            _logger.LogInformation("[DEBUG] Passwort erfolgreich zurückgesetzt, versende Benachrichtigungsmail...");

            // Sende Benachrichtigung, dass das Passwort geändert wurde
            var mailResult = await _mailService.SendPasswordChangedNotificationEmailAsync(user.Email, user.UserName);
            if (mailResult)
            {
                _logger.LogInformation("[DEBUG] Benachrichtigungsmail wurde erfolgreich versendet.");
            }
            else
            {
                _logger.LogError("[ERROR] Beim Versenden der Benachrichtigungsmail ist ein Fehler aufgetreten.");
            }

            return true;
        }



    }
}
