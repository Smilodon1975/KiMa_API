using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using KiMa_API.Models;
using KiMa_API.Services;
using System.Threading.Tasks;
using KiMa_API.Models.Dto;
using Microsoft.Extensions.Logging;

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

        
        /// Konstruktor mit Dependency Injection für Benutzerverwaltung, Authentifizierungs- und JWT-Services.
        
        public AuthController(UserManager<User> userManager, IAuthService authService, JwtService jwtService, ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _authService = authService;
            _jwtService = jwtService;
            _logger = logger;
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

            _logger.LogInformation("Registrierung erfolgreich.");
            return Ok(new { message = "Registrierung erfolgreich!" });
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
            Console.WriteLine($"[DEBUG] Reset-Anfrage für: {model.Email}");
            Console.WriteLine($"[DEBUG] Token: {model.Token}");

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                Console.WriteLine($"[ERROR] Benutzer mit E-Mail {model.Email} nicht gefunden!");
                return NotFound("Benutzer nicht gefunden.");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (!result.Succeeded)
            {
                Console.WriteLine($"[ERROR] Fehler beim Zurücksetzen des Passworts:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($" - {error.Description}");
                }
                return BadRequest(result.Errors);
            }

            return Ok(new { message = "Passwort erfolgreich geändert!" });
        }

    }
}
