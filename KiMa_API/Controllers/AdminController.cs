using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KiMa_API.Data;
using KiMa_API.Models;

namespace KiMa_API.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Nur Admins haben Zugriff
    public class AdminController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;

        public AdminController(UserManager<User> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // 🔹 Alle User abrufen
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }        

        // 🔹 Rolle setzen
        [HttpPost("set-role")]
        public async Task<IActionResult> SetUserRole([FromBody] SetRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null) return NotFound("User nicht gefunden.");

            user.Role = model.Role;
            await _userManager.UpdateAsync(user);
            return Ok("Rolle aktualisiert.");
        }

        // 🔹 User löschen
        [HttpDelete("delete-user/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound("User nicht gefunden.");

            await _userManager.DeleteAsync(user);
            return NoContent();
        }
    }

    public class SetRoleModel
    {
        public int UserId { get; set; }
        public string Role { get; set; }
    }
}
