using KiMa_API.Models;
using KiMa_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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

        // POST api/userprofile
        [HttpPost]
        public async Task<ActionResult<UserProfile>> CreateOrUpdateProfile([FromBody] UserProfile profile)
        {
            var updatedProfile = await _profileService.CreateOrUpdateProfileAsync(profile);
            return Ok(updatedProfile);
        }
    }
}

