using KiMa_API.Data;
using KiMa_API.Models;
using KiMa_API.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KiMa_API.Services
{
    public interface IUserProfileService
    {
        Task<UserProfile> GetProfileByUserIdAsync(int userId);
        Task<UserProfile?> CreateOrUpdateProfileAsync(UserProfileUpdateDto profileDto); // Rückgabetyp ggf. anpassen
    }

    public class UserProfileService : IUserProfileService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserProfileService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor; // Zuweisen
        }

        public async Task<UserProfile> GetProfileByUserIdAsync(int userId)
        {
            return await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<UserProfile?> CreateOrUpdateProfileAsync(UserProfileUpdateDto profileDto)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var currentUserId))
            {
                // Optional: Logging statt nur Exception
                return null; // Oder Exception werfen
            }

            var existingProfile = await GetProfileByUserIdAsync(currentUserId);

            if (existingProfile == null)
            {
                // NEUES UserProfile aus DTO und Claim-ID erstellen
                var newProfile = new UserProfile
                {
                    UserId = currentUserId, // << Wichtig: ID aus Claim setzen
                                            // Daten aus DTO mappen:
                    VehicleCategory = profileDto.VehicleCategory,
                    VehicleDetails = profileDto.VehicleDetails,
                    Occupation = profileDto.Occupation,
                    EducationLevel = profileDto.EducationLevel,
                    Region = profileDto.Region,
                    Age = profileDto.Age,
                    IncomeLevel = profileDto.IncomeLevel,
                    IsInterestedInTechnology = profileDto.IsInterestedInTechnology,
                    IsInterestedInSports = profileDto.IsInterestedInSports,
                    IsInterestedInEntertainment = profileDto.IsInterestedInEntertainment,
                    IsInterestedInTravel = profileDto.IsInterestedInTravel
                };
                _context.UserProfiles.Add(newProfile);
                await _context.SaveChangesAsync();
                return newProfile; // Gib das neue Profil zurück
            }
            else
            {
                // BESTEHENDES Profil mit Daten aus DTO aktualisieren
                existingProfile.VehicleCategory = profileDto.VehicleCategory;
                existingProfile.VehicleDetails = profileDto.VehicleDetails;
                existingProfile.Occupation = profileDto.Occupation;
                existingProfile.EducationLevel = profileDto.EducationLevel;
                existingProfile.Region = profileDto.Region;
                existingProfile.Age = profileDto.Age;
                existingProfile.IncomeLevel = profileDto.IncomeLevel;
                existingProfile.IsInterestedInTechnology = profileDto.IsInterestedInTechnology;
                existingProfile.IsInterestedInSports = profileDto.IsInterestedInSports;
                existingProfile.IsInterestedInEntertainment = profileDto.IsInterestedInEntertainment;
                existingProfile.IsInterestedInTravel = profileDto.IsInterestedInTravel;

                _context.UserProfiles.Update(existingProfile);
                await _context.SaveChangesAsync();
                return existingProfile; // Gib das aktualisierte Profil zurück
            }
        }
    }
}

