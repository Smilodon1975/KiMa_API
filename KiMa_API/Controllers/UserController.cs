using KiMa_API.Models; // <-- Importiere UserUpdateDto
using KiMa_API.Services; // <-- Importiere UserService
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KiMa_API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;

        public UserController(IUserService userService, UserManager<User> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user == null ? NotFound("User nicht gefunden.") : Ok(user);
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto userUpdateDto)
        {
            var requestUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var requestUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var success = await _userService.UpdateUserAsync(userUpdateDto, requestUserId, requestUserRole ?? "Proband");
            if (!success) return BadRequest("Fehler beim Speichern der Daten.");

            return Ok("Änderungen erfolgreich gespeichert!");
        }
    }
}
