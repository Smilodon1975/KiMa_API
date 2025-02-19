using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using KiMa_API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KiMa_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;

        public AuthController(UserManager<User> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        // 🔹 User-Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized("Falsche E-Mail oder Passwort.");

            var token = GenerateJwtToken(user);
            Console.WriteLine($"Generated Token: {token}");
            return Ok(new { token });
        }

        // 🔹 User-Registrierung
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            Console.WriteLine("[DEBUG] Registrierung gestartet für " + model.Email);

            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                Console.WriteLine("[ERROR] E-Mail bereits registriert: " + model.Email);
                return BadRequest("Diese E-Mail wird bereits verwendet!");
            }

            var user = new User
            {
                Email = model.Email,
                NormalizedEmail = model.Email.ToUpper(), // Normalized Email für Identity
                FirstName = model.FirstName ?? "",
                LastName = model.LastName ?? "",
                Phone = model.Phone ?? "",
                Age = model.Age,
                CreatedAt = DateTime.UtcNow // Timestamp setzen
            };

            try
            {
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    Console.WriteLine("[ERROR] Fehler beim Erstellen des Users:");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"- {error.Code}: {error.Description}");
                    }
                    return BadRequest(result.Errors);
                }

                Console.WriteLine("[SUCCESS] User erfolgreich registriert: " + model.Email);
                return Ok(new { message = "User erfolgreich registriert." });

            }
            catch (Exception ex)
            {
                Console.WriteLine("[FATAL ERROR] Ausnahme aufgetreten: " + ex.Message);
                return StatusCode(500, "Interner Serverfehler: " + ex.Message);
            }
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role ?? "Proband")
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
