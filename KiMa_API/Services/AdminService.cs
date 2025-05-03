using KiMa_API.Data;
using KiMa_API.Models;
using KiMa_API.Models.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KiMa_API.Services
{

    /// Service für administrative Aufgaben, einschließlich Benutzerverwaltung und Rollenmanagement.

    public class AdminService : IAdminService
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;


        /// Konstruktor zur Initialisierung des AdminService mit `UserManager` und `AppDbContext`.

        public AdminService(UserManager<User> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        /// Ruft eine Liste aller Benutzer aus der Datenbank ab.        

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }


        /// Löscht einen Benutzer anhand seiner ID.       
        /// <returns>True, wenn der Benutzer erfolgreich gelöscht wurde, andernfalls False.</returns>
        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }


        /// Weist einem Benutzer eine neue Rolle zu um neuen Admin einzurichten        
        /// <returns>True, wenn die Rollenänderung erfolgreich war, andernfalls False.</returns>
        public async Task<bool> SetUserRoleAsync(int userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            user.Role = newRole;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }


        /// Aktualisiert die Benutzerdaten eines bestehenden Benutzers.       
        /// <returns>True, wenn die Aktualisierung erfolgreich war, andernfalls False.</returns>
        public async Task<bool> UpdateUserAsync(UserUpdateDto updateDto)
        {
            var user = await _userManager.FindByIdAsync(updateDto.Id.ToString());
            if (user == null) return false;

            // 🔹 Benutzerdaten aktualisieren, falls neue Werte vorhanden sind
            user.UserName = updateDto.UserName ?? user.UserName;
            user.Email = updateDto.Email ?? user.Email;
            user.FirstName = updateDto.FirstName ?? user.FirstName;
            user.LastName = updateDto.LastName ?? user.LastName;
            user.Title = updateDto.Title ?? user.Title;
            user.Gender = updateDto.Gender ?? user.Gender;
            user.Status = updateDto.Status ?? user.Status;
            user.PhonePrivate = updateDto.PhonePrivate ?? user.PhonePrivate;
            user.PhoneMobile = updateDto.PhoneMobile ?? user.PhoneMobile;
            user.PhoneWork = updateDto.PhoneWork ?? user.PhoneWork;
            user.Age = updateDto.Age ?? user.Age;
            user.BirthDate = updateDto.BirthDate ?? user.BirthDate;
            user.Street = updateDto.Street ?? user.Street;
            user.Zip = updateDto.Zip ?? user.Zip;
            user.City = updateDto.City ?? user.City;
            user.Country = updateDto.Country ?? user.Country;
            user.DataConsent = updateDto.DataConsent ?? user.DataConsent;

            if (!string.IsNullOrEmpty(updateDto.Password))
            {
                var passwordHasher = new PasswordHasher<User>();
                user.PasswordHash = passwordHasher.HashPassword(user, updateDto.Password);
            }


            // 🔹 Falls die Rolle geändert wurde, aktualisieren
            if (!string.IsNullOrEmpty(updateDto.Role) && user.Role != updateDto.Role)
            {
                user.Role = updateDto.Role;
            }

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}
