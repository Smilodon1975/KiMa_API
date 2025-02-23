using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KiMa_API.Data;
using KiMa_API.Models;
using KiMa_API.Services;

namespace KiMa_API.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Nur Admins haben Zugriff
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // 🔹 Alle User abrufen
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return Ok(users);
        }

       
        // 🔹 User löschen
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _adminService.DeleteUserAsync(id);
            if (!success) return NotFound("Benutzer nicht gefunden oder konnte nicht gelöscht werden.");
            return Ok("Benutzer erfolgreich gelöscht.");
        }
    

    [HttpPut("set-role/{id}")]
        public async Task<IActionResult> SetUserRole(int id, [FromBody] string newRole)
        {
            var success = await _adminService.SetUserRoleAsync(id, newRole);
            if (!success) return NotFound("Benutzer nicht gefunden oder Fehler beim Setzen der Rolle.");
            return Ok("Rolle erfolgreich aktualisiert.");
        }
    }

    public class SetRoleModel
    {
        public int UserId { get; set; }
        public string Role { get; set; }
    }
}
