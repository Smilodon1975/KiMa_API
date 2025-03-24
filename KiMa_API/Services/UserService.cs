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
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == requestUserId);
        }

        // Ruft eine Liste aller Benutzer ab.
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        // Aktualisiert die Benutzerdaten eines bestehenden Benutzers.
        public async Task<bool> UpdateUserAsync(UserUpdateModel updateModel, int requestUserId, string requestUserRole)
        {
            var user = await _context.Users.FindAsync(updateModel.Id);
            if (user == null)
                return false;

            // Aktualisiere nur Felder, die nicht null oder leer sind
            if (!string.IsNullOrWhiteSpace(updateModel.UserName)) user.UserName = updateModel.UserName;
            if (!string.IsNullOrWhiteSpace(updateModel.Email)) user.Email = updateModel.Email;
            if (!string.IsNullOrWhiteSpace(updateModel.FirstName)) user.FirstName = updateModel.FirstName;
            if (!string.IsNullOrWhiteSpace(updateModel.LastName)) user.LastName = updateModel.LastName;
            if (!string.IsNullOrWhiteSpace(updateModel.Title)) user.Title = updateModel.Title;
            if (!string.IsNullOrWhiteSpace(updateModel.Gender)) user.Gender = updateModel.Gender;
            if (!string.IsNullOrWhiteSpace(updateModel.Status)) user.Status = updateModel.Status;

            if (!string.IsNullOrWhiteSpace(updateModel.PhonePrivate)) user.PhonePrivate = updateModel.PhonePrivate;
            if (!string.IsNullOrWhiteSpace(updateModel.PhoneMobile)) user.PhoneMobile = updateModel.PhoneMobile;
            if (!string.IsNullOrWhiteSpace(updateModel.PhoneWork)) user.PhoneWork = updateModel.PhoneWork;

            if (updateModel.Age.HasValue) user.Age = updateModel.Age.Value;
            if (updateModel.BirthDate.HasValue) user.BirthDate = updateModel.BirthDate.Value;

            if (!string.IsNullOrWhiteSpace(updateModel.Street)) user.Street = updateModel.Street;
            if (!string.IsNullOrWhiteSpace(updateModel.Zip)) user.Zip = updateModel.Zip;
            if (!string.IsNullOrWhiteSpace(updateModel.City)) user.City = updateModel.City;
            if (!string.IsNullOrWhiteSpace(updateModel.Country)) user.Country = updateModel.Country;

            if (!string.IsNullOrEmpty(updateModel.Password))
            {
                var passwordHasher = new PasswordHasher<User>();
                user.PasswordHash = passwordHasher.HashPassword(user, updateModel.Password);
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
