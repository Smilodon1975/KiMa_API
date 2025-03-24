using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using KiMa_API.Models;
using KiMa_API.Services;
using System.Threading.Tasks;
using KiMa_API.Models.Dto;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace KiMa_API.Controllers
{
 
    /// Der AuthController verwaltet die Authentifizierungsprozesse wie Login, Registrierung und Passwort-Reset.
    
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuthService _authService;
        private readonly JwtService _jwtService;
        private readonly ILogger<AuthService> _logger;
        private readonly IMailService _mailService;

        
        /// Konstruktor mit Dependency Injection für Benutzerverwaltung, Authentifizierungs- und JWT-Services.
        
        public AuthController(UserManager<User> userManager, IAuthService authService, JwtService jwtService, ILogger<AuthService> logger, IMailService mailService)
        {
            _userManager = userManager;
            _authService = authService;
            _jwtService = jwtService;
            _logger = logger;
            _mailService = mailService;
        }

      
        /// Login für Benutzer. Prüft die Anmeldedaten und gibt ein JWT-Token zurück.
       
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized("Falsche E-Mail oder Passwort.");

            var token = _jwtService.GenerateJwtToken(user); // Token für die Sitzung generieren
            return Ok(new { token });
        }


        /// Registriert einen neuen Benutzer und gibt eine Erfolgsmeldung zurück.

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            _logger.LogInformation("Registrierungsversuch gestartet.");

            // Validierung prüfen
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogWarning("Registrierung fehlgeschlagen: Validierungsfehler.");
                return BadRequest(new { message = "Validierungsfehler", errors });
            }

            // Manuelle E-Mail-Format-Prüfung
            var emailValidator = new EmailAddressAttribute();
            if (!emailValidator.IsValid(model.Email))
            {
                _logger.LogWarning("Registrierung fehlgeschlagen: Ungültige E-Mail-Adresse.");
                return BadRequest(new { message = "Ungültige E-Mail-Adresse." });
            }

            var result = await _authService.RegisterAsync(model);
            if (result == null)
            {
                _logger.LogError("Fehler: Authentifizierungsservice hat null zurückgegeben.");
                return StatusCode(500, "Interner Serverfehler.");
            }

            // Falls Registrierung fehlschlägt, Fehler zurückgeben
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                _logger.LogError("Registrierung fehlgeschlagen.");
                foreach (var error in result.Errors)
                {
                    _logger.LogError($"Fehler: {error.Code} - {error.Description}");
                }
                return BadRequest(new { message = "Registrierung fehlgeschlagen", errors });
            }

            // Registrierung war erfolgreich – nun Bestätigungs-Mail senden
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await _mailService.SendEmailConfirmationEmailAsync(user.Email, confirmationToken, user.UserName);
            }

            _logger.LogInformation("Registrierung erfolgreich.");
            return Ok(new { message = "Registrierung erfolgreich! Bitte bestätige deine E-Mail-Adresse." });
        }




        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
        {
            _logger.LogInformation($"ConfirmEmail aufgerufen mit Email: {email} und Token: {token}");

            // Manuelles Decoding des Tokens und Wiederherstellen des Pluszeichens
            var decodedToken = System.Net.WebUtility.UrlDecode(token);
            // Ersetze Leerzeichen durch Pluszeichen, falls sie fälschlicherweise entstanden sind
            decodedToken = decodedToken.Replace(" ", "+");
            _logger.LogInformation($"Decoded Token: {decodedToken}");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogError("Benutzer nicht gefunden.");
                return NotFound("Benutzer nicht gefunden.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (result.Succeeded)
            {
                _logger.LogInformation("E-Mail erfolgreich bestätigt.");
                return Ok("E-Mail erfolgreich bestätigt!");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError($"E-Mail-Bestätigung Fehler: {error.Code} - {error.Description}");
                }
                return BadRequest("E-Mail-Bestätigung fehlgeschlagen.");
            }
        }








        /// Fordert einen Passwort-Reset an und sendet einen Reset-Link.

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDto model)
        {
            var token = await _authService.GeneratePasswordResetTokenAsync(model.Email);
            if (token == null)
                return NotFound("E-Mail nicht gefunden.");

            return Ok(new { message = "Passwort-Reset-Link wurde gesendet." });
        }


        /// Setzt das Passwort anhand des übergebenen Tokens zurück.

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDto model)
        {
            _logger.LogInformation($"[DEBUG] Reset-Anfrage für: {model.Email}");
            _logger.LogInformation($"[DEBUG] Token: {model.Token}");

            // Statt direkt _userManager.ResetPasswordAsync aufzurufen,
            // rufen wir die Methode im AuthService auf, die auch die Mail sendet.
            var result = await _authService.ResetPasswordAsync(model);

            if (!result)
            {
                _logger.LogError("Fehler beim Zurücksetzen des Passworts oder Senden der Benachrichtigung.");
                return BadRequest("Passwort konnte nicht zurückgesetzt werden.");
            }

            return Ok(new { message = "Passwort erfolgreich geändert!" });
        }


    }
}
