using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using KiMa_API.Models;
using KiMa_API.Services;
using System.Threading.Tasks;
using KiMa_API.Models.Dto;
using Microsoft.Extensions.Logging;

namespace KiMa_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuthService _authService;
        private readonly JwtService _jwtService;
        private readonly ILogger<AuthService> _logger;

        public AuthController(UserManager<User> userManager, IAuthService authService, JwtService jwtService, ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _authService = authService;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized("Falsche E-Mail oder Passwort.");

            var token = _jwtService.GenerateJwtToken(user); // 👈 Token über Service generieren
            return Ok(new { token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            _logger.LogInformation("DEBUG: Register-Endpoint wurde aufgerufen!");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage);
                _logger.LogWarning("DEBUG: Validierungsfehler beim Registrieren!");
                return BadRequest(new { message = "Validierungsfehler", errors });
            }

            var result = await _authService.RegisterAsync(model);

            if (result == null)
            {
                _logger.LogError("DEBUG: _authService.RegisterAsync() hat null zurückgegeben!");
            }

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                _logger.LogError("DEBUG: Registrierung fehlgeschlagen!");
                foreach (var error in result.Errors)
                {
                    _logger.LogError($"DEBUG: {error.Code} - {error.Description}");
                }
                return BadRequest(new { message = "Registrierung fehlgeschlagen", errors });
            }

            _logger.LogInformation("DEBUG: Registrierung erfolgreich!");
            return Ok(new { message = "Registrierung erfolgreich!" });
        }



        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDto model)
        {
            var token = await _authService.GeneratePasswordResetTokenAsync(model.Email);
            if (token == null) return NotFound("E-Mail nicht gefunden.");

            return Ok(new { message = "Passwort-Reset-Link wurde gesendet." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDto model)
        {
            var success = await _authService.ResetPasswordAsync(model);
            if (!success) return BadRequest("Passwort konnte nicht zurückgesetzt werden.");

            return Ok(new { message = "Passwort erfolgreich geändert!" });
        }




    }
}
