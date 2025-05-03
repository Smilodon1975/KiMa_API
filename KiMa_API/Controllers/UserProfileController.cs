using KiMa_API.Models;
using KiMa_API.Models.Dto;
using KiMa_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KiMa_API.Controllers
{
    [Route("api/userprofile")]
    [ApiController]
    [Authorize] // Nur authentifizierte Benutzer dürfen auf ihr Profil zugreifen
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _profileService;

        public UserProfileController(IUserProfileService profileService)
        {
            _profileService = profileService;
        }

        // GET api/userprofile/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<UserProfile>> GetProfile(int userId)
        {
            var profile = await _profileService.GetProfileByUserIdAsync(userId);
            if (profile == null)
                return NotFound();
            return Ok(profile);
        }


        // Ändere hier UserProfile zu UserProfileUpdateDto
        [HttpPut]
        public async Task<ActionResult<UserProfile>> CreateOrUpdateProfile([FromBody] UserProfileUpdateDto profileDto)
        {
            // ---> NEU: ModelState überprüfen <---
            if (!ModelState.IsValid)
            {
                // Gibt die detaillierten Validierungsfehler zurück
                return BadRequest(ModelState);
            }
            // ---> Ende der neuen Prüfung <---

            try // Optional: Try-Catch für Service-Fehler
            {
                var updatedProfile = await _profileService.CreateOrUpdateProfileAsync(profileDto);
                if (updatedProfile == null)
                {
                    // Service hat signalisiert, dass etwas schief ging (z.B. User nicht gefunden)
                    return BadRequest("Profil konnte nicht erstellt/aktualisiert werden, Service-Fehler.");
                }
                return Ok(updatedProfile);
            }
            catch (Exception ex) // Fängt unerwartete Fehler im Service ab
            {
                // Logge den Fehler serverseitig
                // _logger.LogError(ex, "Fehler in CreateOrUpdateProfileAsync");
                return StatusCode(StatusCodes.Status500InternalServerError, "Ein interner Fehler ist aufgetreten.");
            }
        }
    }
}

