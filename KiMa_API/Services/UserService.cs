using KiMa_API.Data;
using KiMa_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static KiMa_API.Controllers.UserController;

namespace KiMa_API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;

        public UserService(UserManager<User> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetMyDataAsync(int requestUserId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == requestUserId);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<bool> UpdateUserAsync(UserUpdateDto userUpdateDto, int requestUserId, string requestUserRole)
        {
            var user = await _userManager.FindByIdAsync(userUpdateDto.Id.ToString());
            if (user == null) return false;

            if (requestUserRole != "Admin" && user.Id != requestUserId)
                return false;

            user.FirstName = userUpdateDto.FirstName ?? user.FirstName;
            user.LastName = userUpdateDto.LastName ?? user.LastName;
            user.PhoneMobile = userUpdateDto.PhoneMobile ?? user.PhoneMobile;
            user.PhonePrivate = userUpdateDto.PhonePrivate ?? user.PhonePrivate;
            user.PhoneWork = userUpdateDto.PhoneWork ?? user.PhoneWork;
            user.Age = userUpdateDto.Age ?? user.Age;
            user.Street = userUpdateDto.Street ?? user.Street;
            user.Zip = userUpdateDto.Zip ?? user.Zip;
            user.City = userUpdateDto.City ?? user.City;
            user.Country = userUpdateDto.Country ?? user.Country;
            user.BirthDate = userUpdateDto.BirthDate ?? user.BirthDate;
            user.Gender = userUpdateDto.Gender ?? user.Gender;
            user.Title = userUpdateDto.Title ?? user.Title;
            user.Status = userUpdateDto.Status ?? user.Status;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}

