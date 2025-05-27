
using KiMa_API.Models;
using KiMa_API.Models.Dto;
using KiMa_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace KiMa_API.Controllers
{

    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;             

         
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }
               
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return Ok(users);
        }
        
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _adminService.DeleteUserAsync(id);
            if (!success)
                return NotFound(new { message = "Benutzer nicht gefunden oder konnte nicht gelöscht werden." });

            return NoContent();
        }

       
        [HttpPut("set-role/{id}")]
        public async Task<IActionResult> SetUserRole(int id, [FromBody] string newRole)
        {
            var success = await _adminService.SetUserRoleAsync(id, newRole);
            if (!success)
                return NotFound("Benutzer nicht gefunden oder Fehler beim Setzen der Rolle.");

            return Ok("Rolle erfolgreich aktualisiert.");
        }
      
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto updateDto)
        {
            var success = await _adminService.UpdateUserAsync(updateDto);
            if (!success)
                return BadRequest(new { message = "Fehler beim Speichern der Änderungen." });

            return Ok(new { message = "Benutzer erfolgreich aktualisiert!" });
        }        
    }
    public class SetRoleModel
    {
        public int UserId { get; set; }
        public string Role { get; set; } = "Proband"; // Fix: Initialize the property with a default value
    }
}