using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using KiMa_API.Models;
using KiMa_API.Services;
using System.Threading.Tasks;

namespace KiMa_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly AuthService _authService;  // 🛠 Fix: _authService hinzugefügt!
        private readonly JwtService _jwtService;

        public AuthController(UserManager<User> userManager, AuthService authService, JwtService jwtService)
        {
            _userManager = userManager;
            _authService = authService;  // 🛠 Fix: Richtige Initialisierung!
            _jwtService = jwtService;
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
            var success = await _authService.RegisterAsync(model); // 🛠 Fix: Jetzt korrekt auf _authService zugreifen!
            if (!success) return BadRequest("Registrierung fehlgeschlagen.");

            return Ok(new { message = "User erfolgreich registriert." });
        }
    }
}
