using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using KiMa_API.Data;
using KiMa_API.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KiMa_API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;

        public UserController(UserManager<User> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // 🔹 Alle User abrufen (nur Admins)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }


        [Authorize]
        [HttpGet("user-role")]
        public async Task<IActionResult> GetUserRole()
        {
            Console.WriteLine("[DEBUG] `GetUserRole()` aufgerufen.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("[ERROR] Kein Benutzer-Token gefunden!");
                return Unauthorized("Kein Benutzer-Token gefunden.");
            }

            Console.WriteLine($"[DEBUG] Extrahierte Benutzer-ID: {userId}");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                Console.WriteLine($"[ERROR] Benutzer mit ID {userId} nicht gefunden!");
                return NotFound("User nicht gefunden.");
            }

            Console.WriteLine($"[DEBUG] Ermittelte Benutzerrolle: {user.Role}");
            return Ok(new { role = user.Role ?? "Proband" });
        }






        // 🔹 Einzelnen User abrufen (Admin: beliebig, User: nur eigene Daten)
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return NotFound("User nicht gefunden.");

            if (userRole != "Admin" && user.Id != userId)
                return Forbid(); // User dürfen nur ihre eigenen Daten sehen

            return Ok(user);
        }

        [Authorize]
        [HttpGet("me")] // 🔥 Neue Route für eigene User-Daten
        public async Task<IActionResult> GetMyData()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized("Kein Benutzer-Token gefunden.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == int.Parse(userId));
            if (user == null) return NotFound("User nicht gefunden.");

            return Ok(user);
        }



    }
}

