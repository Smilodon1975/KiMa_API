using System.Security.Claims;
using KiMa_API.Models;
using KiMa_API.Models.Dto;
using KiMa_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace KiMa_API.Controllers
{

    /// Der UserController verwaltet Benutzeraktionen wie das Abrufen und Aktualisieren der eigenen Daten.
    /// Einige Endpunkte sind Admin-exklusiv.

    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AuthService> _logger;
        private readonly IMailService _mailService;


        /// Konstruktor mit Dependency Injection für Benutzerverwaltung und Logging.

        public UserController(IUserService userService, UserManager<User> userManager, ILogger<AuthService> logger, IMailService mailService)
        {
            _userService = userService;
            _userManager = userManager;
            _logger = logger;
            _mailService = mailService;
        }


        /// Gibt die Rolle des aktuell eingeloggten Benutzers zurück.

        [HttpGet("user-role")]
        [Authorize]
        public async Task<IActionResult> GetUserRole()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null) return NotFound("User nicht gefunden.");

            return Ok(new { role = user.Role });
        }


        /// Ruft die eigenen Benutzerdaten basierend auf dem JWT-Token ab.

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyData()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                _logger.LogWarning("Kein Benutzer-ID-Claim gefunden!");
                return Unauthorized("Kein gültiges Token.");
            }

            // Verwende TryParse für mehr Sicherheit
            if (!int.TryParse(userIdClaim.Value, out var userId))
            {
                _logger.LogWarning($"Ungültiger Benutzer-ID-Claim: {userIdClaim.Value}");
                return BadRequest("Ungültige Benutzer-ID im Token.");
            }

            _logger.LogInformation($"Abruf der eigenen Daten für UserID: {userId}");

            // ---> KORREKTUR: Rufe jetzt GetMyDataAsync auf <---
            var user = await _userService.GetMyDataAsync(userId);

            if (user == null)
            {
                _logger.LogWarning($"Benutzer mit ID {userId} nicht gefunden!");
                return NotFound("Benutzer nicht gefunden.");
            }

            // 'user' enthält jetzt auch das UserProfile (wenn in DB vorhanden)
            return Ok(user);
        }


        /// Ruft eine Liste aller Benutzer ab (nur für Admins).

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }


        /// Ruft einen Benutzer anhand der ID ab.

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user == null ? NotFound("User nicht gefunden.") : Ok(user);
        }


        /// Aktualisiert die Benutzerdaten. Benutzer können nur ihre eigenen Daten ändern, 
        /// Admins können Änderungen für alle Benutzer durchführen.

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto updateDto)
        {
            var requestUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var requestUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var success = await _userService.UpdateUserAsync(updateDto, requestUserId, requestUserRole ?? "Proband");
            if (!success)
            {
                return BadRequest("Fehler beim Speichern der Daten.");
            }

            // Wenn ein neues Passwort angegeben wurde, versende eine Benachrichtigungsemail
            if (!string.IsNullOrWhiteSpace(updateDto.Password))
            {
                // Benutzer erneut abrufen, um Email und UserName zu erhalten
                var user = await _userService.GetUserByIdAsync(requestUserId);
                if (user != null)
                {
                    await _mailService.SendPasswordChangedNotificationEmailAsync(user.Email, user.UserName);
                }
            }

            return Ok(new { message = "Änderungen erfolgreich gespeichert!" });
        }


        // Neuer Endpunkt zum Löschen des Accounts
        [HttpPost("delete-account")]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountDto dto)
        {
            // Hole die User-ID aus den Claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Ungültige Benutzer-ID.");
            }

            var result = await _userService.DeleteAccountAsync(userId, dto.Password);
            if (result)
            {
                return Ok(new { message = "Account erfolgreich gelöscht." });
            }
            else
            {
                return BadRequest("Löschen des Accounts fehlgeschlagen. Überprüfe dein Passwort.");
            }
        }

    }
}
