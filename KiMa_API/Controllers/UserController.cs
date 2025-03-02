using KiMa_API.Models; 
using KiMa_API.Services; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using KiMa_API.Models.Dto;
using Microsoft.Extensions.Logging;

namespace KiMa_API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AuthService> _logger;

        public UserController(IUserService userService, UserManager<User> userManager, ILogger<AuthService> logger)
        {
            _userService = userService;
            _userManager = userManager;
            _logger = logger;
        }

        // ✅ Eigene Benutzerdaten abrufen
        [HttpGet("user-role")]
        [Authorize]
        public async Task<IActionResult> GetUserRole()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null) return NotFound("User nicht gefunden.");

            return Ok(new { role = user.Role });
        }


        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyData()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                _logger.LogWarning("DEBUG: Kein Benutzer-ID-Claim gefunden!");
                return Unauthorized("Kein gültiges Token.");
            }

            var userId = int.Parse(userIdClaim.Value);
            _logger.LogInformation($"DEBUG: GetMyData() aufgerufen für UserID: {userId}");

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning($"DEBUG: Benutzer mit ID {userId} nicht gefunden!");
                return NotFound("Benutzer nicht gefunden.");
            }

            return Ok(user);
        }


        // ✅ Alle Benutzer abrufen (Nur für Admins)
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }


        // ✅ Benutzer per ID abrufen
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user == null ? NotFound("User nicht gefunden.") : Ok(user);
        }


        // ✅ Benutzer aktualisieren
        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateModel updateModel)
        {
            var requestUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var requestUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var success = await _userService.UpdateUserAsync(updateModel, requestUserId, requestUserRole ?? "Proband");
            if (!success) return BadRequest("Fehler beim Speichern der Daten.");

            return Ok(new { message = "Änderungen erfolgreich gespeichert!" });
        }
    }
}
