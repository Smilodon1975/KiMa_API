using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using KiMa_API.Models;
using KiMa_API.Models.Dto;
using KiMa_API.Services;
using System.Threading.Tasks;

namespace KiMa_API.Controllers
{
    
    /// Der AdminController verwaltet administrative Aufgaben wie das Abrufen, Aktualisieren, Löschen 
    /// und Setzen von Benutzerrollen. Alle Endpunkte sind nur für Benutzer mit der Rolle "Admin" zugänglich.
    
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Nur Admins haben Zugriff
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService; // Service zur Verwaltung von Admin-Funktionen
        private readonly UserManager<User> _userManager; // ASP.NET Identity-UserManager zur Verwaltung der Benutzer

        
        /// Konstruktor zur Injektion der benötigten Dienste.  
              public AdminController(IAdminService adminService, UserManager<User> userManager)
        {
            _adminService = adminService;
            _userManager = userManager;
        }

       
        /// Ruft eine Liste aller registrierten Benutzer ab.        
        /// <returns>Eine Liste von Benutzern im JSON-Format</returns>
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return Ok(users);
        }

       
        /// Löscht einen Benutzer anhand der ID.        
        /// <returns>Eine Bestätigung oder eine Fehlermeldung</returns>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _adminService.DeleteUserAsync(id);
            if (!success)
                return NotFound("Benutzer nicht gefunden oder konnte nicht gelöscht werden.");

            return Ok("Benutzer erfolgreich gelöscht.");
        }

       
        /// Setzt eine neue Rolle für einen bestimmten Benutzer.        
        /// <param name="id">Die ID des Benutzers</param>
        /// <param name="newRole">Die neue Rolle, die zugewiesen werden soll</param>
        /// <returns>Eine Bestätigung oder eine Fehlermeldung</returns>
        [HttpPut("set-role/{id}")]
        public async Task<IActionResult> SetUserRole(int id, [FromBody] string newRole)
        {
            var success = await _adminService.SetUserRoleAsync(id, newRole);
            if (!success)
                return NotFound("Benutzer nicht gefunden oder Fehler beim Setzen der Rolle.");

            return Ok("Rolle erfolgreich aktualisiert.");
        }

       
        /// Aktualisiert die Benutzerdaten eines bestehenden Benutzers.        
        /// <returns>Eine Bestätigung oder eine Fehlermeldung</returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateModel userUpdate)
        {
            var success = await _adminService.UpdateUserAsync(userUpdate);
            if (!success)
                return BadRequest(new { message = "Fehler beim Speichern der Änderungen." });

            return Ok(new { message = "Benutzer erfolgreich aktualisiert!" });
        }
    }


    /// Modell zur Repräsentation einer Benutzerrollenänderung.  
    public class SetRoleModel
    {
        public int UserId { get; set; } // Die Benutzer-ID, für die die Rolle geändert werden soll
        public string Role { get; set; } // Die neue Rolle des Benutzers
    }
}
