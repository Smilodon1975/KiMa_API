using KiMa_API.Data;
using KiMa_API.Models;
using KiMa_API.Models.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static KiMa_API.Controllers.UserController;

namespace KiMa_API.Services
{
    // Service für Benutzerverwaltung, einschließlich Datenabfrage und Aktualisierung.
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;

        public UserService(UserManager<User> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Ruft einen Benutzer anhand der ID ab.
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        // Ruft die eigenen Benutzerdaten basierend auf der Anfrage-ID ab.
        public async Task<User?> GetMyDataAsync(int requestUserId)
        {
            return await _context.Users
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Id == requestUserId);
        }


        // Ruft eine Liste aller Benutzer ab.
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        // Aktualisiert die Benutzerdaten eines bestehenden Benutzers.
        public async Task<bool> UpdateUserAsync(UserUpdateDto updateDto, int requestUserId, string requestUserRole)
        {
            var user = await _context.Users.FindAsync(updateDto.Id);
            if (user == null)
                return false;

            // Aktualisiere nur Felder, die nicht null oder leer sind
            if (!string.IsNullOrWhiteSpace(updateDto.UserName)) user.UserName = updateDto.UserName;
            if (!string.IsNullOrWhiteSpace(updateDto.Email)) user.Email = updateDto.Email;
            if (!string.IsNullOrWhiteSpace(updateDto.FirstName)) user.FirstName = updateDto.FirstName;
            if (!string.IsNullOrWhiteSpace(updateDto.LastName)) user.LastName = updateDto.LastName;
            if (!string.IsNullOrWhiteSpace(updateDto.Title)) user.Title = updateDto.Title;
            if (!string.IsNullOrWhiteSpace(updateDto.Gender)) user.Gender = updateDto.Gender;
            if (!string.IsNullOrWhiteSpace(updateDto.Status)) user.Status = updateDto.Status;

            if (!string.IsNullOrWhiteSpace(updateDto.PhonePrivate)) user.PhonePrivate = updateDto.PhonePrivate;
            if (!string.IsNullOrWhiteSpace(updateDto.PhoneMobile)) user.PhoneMobile = updateDto.PhoneMobile;
            if (!string.IsNullOrWhiteSpace(updateDto.PhoneWork)) user.PhoneWork = updateDto.PhoneWork;

            if (updateDto.Age.HasValue) user.Age = updateDto.Age.Value;
            if (updateDto.BirthDate.HasValue) user.BirthDate = updateDto.BirthDate.Value;

            if (!string.IsNullOrWhiteSpace(updateDto.Street)) user.Street = updateDto.Street;
            if (!string.IsNullOrWhiteSpace(updateDto.Zip)) user.Zip = updateDto.Zip;
            if (!string.IsNullOrWhiteSpace(updateDto.City)) user.City =   updateDto.City;
            if (!string.IsNullOrWhiteSpace(updateDto.Country)) user.Country = updateDto.Country;

            if (!string.IsNullOrEmpty(updateDto.Password))
            {
                var passwordHasher = new PasswordHasher<User>();
                user.PasswordHash = passwordHasher.HashPassword(user, updateDto.Password);
            }
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }



        public async Task<bool> DeleteAccountAsync(int userId, string password)
        {
            // Benutzer über den UserManager abrufen (Convert int to string, falls nötig)
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return false;

            // Überprüfe, ob das eingegebene Passwort korrekt ist
            var passwordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!passwordValid)
                return false;

            // Lösche den Benutzer
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

    }
}
