using KiMa_API.Data;
using KiMa_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using KiMa_API.Models.Dto;

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
        public async Task<bool> UpdateUserAsync(UserUpdateModel userUpdate)
        {
            var user = await _userManager.FindByIdAsync(userUpdate.Id.ToString());
            if (user == null) return false;

            // 🔹 Benutzerdaten aktualisieren, falls neue Werte vorhanden sind
            user.UserName = userUpdate.UserName ?? user.UserName;
            user.FirstName = userUpdate.FirstName ?? user.FirstName;
            user.LastName = userUpdate.LastName ?? user.LastName;
            user.Email = userUpdate.Email ?? user.Email;
            user.Title = userUpdate.Title ?? user.Title;
            user.Gender = userUpdate.Gender ?? user.Gender;
            user.Status = userUpdate.Status ?? user.Status;
            user.PhonePrivate = userUpdate.PhonePrivate ?? user.PhonePrivate;
            user.PhoneMobile = userUpdate.PhoneMobile ?? user.PhoneMobile;
            user.PhoneWork = userUpdate.PhoneWork ?? user.PhoneWork;
            user.Age = userUpdate.Age ?? user.Age;
            user.BirthDate = userUpdate.BirthDate ?? user.BirthDate;
            user.Street = userUpdate.Street ?? user.Street;
            user.Zip = userUpdate.Zip ?? user.Zip;
            user.City = userUpdate.City ?? user.City;
            user.Country = userUpdate.Country ?? user.Country;

            // 🔹 Falls die Rolle geändert wurde, aktualisieren
            if (!string.IsNullOrEmpty(userUpdate.Role) && user.Role != userUpdate.Role)
            {
                user.Role = userUpdate.Role;
            }

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}
