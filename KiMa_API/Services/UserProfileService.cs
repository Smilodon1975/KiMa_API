using KiMa_API.Data;
using KiMa_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KiMa_API.Services
{
    public interface IUserProfileService
    {
        Task<UserProfile> GetProfileByUserIdAsync(int userId);
        Task<UserProfile> CreateOrUpdateProfileAsync(UserProfile profile);
    }

    public class UserProfileService : IUserProfileService
    {
        private readonly AppDbContext _context;

        public UserProfileService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfile> GetProfileByUserIdAsync(int userId)
        {
            return await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<UserProfile> CreateOrUpdateProfileAsync(UserProfile profile)
        {
            var existingProfile = await GetProfileByUserIdAsync(profile.UserId);
            if (existingProfile == null)
            {
                _context.UserProfiles.Add(profile);
            }
            else
            {
                // Update vorhandenes Profil
                existingProfile.VehicleCategory = profile.VehicleCategory;
                existingProfile.VehicleDetails = profile.VehicleDetails;
                existingProfile.Occupation = profile.Occupation;
                existingProfile.EducationLevel = profile.EducationLevel;
                existingProfile.Region = profile.Region;
                existingProfile.Age = profile.Age;
                existingProfile.IncomeLevel = profile.IncomeLevel;
                existingProfile.IsInterestedInTechnology = profile.IsInterestedInTechnology;
                existingProfile.IsInterestedInSports = profile.IsInterestedInSports;
                existingProfile.IsInterestedInEntertainment = profile.IsInterestedInEntertainment;
                existingProfile.IsInterestedInTravel = profile.IsInterestedInTravel;
                _context.UserProfiles.Update(existingProfile);
            }

            await _context.SaveChangesAsync();
            return profile;
        }
    }
}

