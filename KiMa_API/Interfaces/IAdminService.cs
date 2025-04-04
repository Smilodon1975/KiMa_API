using KiMa_API.Models;
using KiMa_API.Models.Dto;

namespace KiMa_API.Services
{
 
    /// Schnittstelle für administrative Benutzerverwaltungsfunktionen.  
    public interface IAdminService
    {
     
        /// Ruft eine Liste aller Benutzer aus der Datenbank ab.        
        /// <returns>Eine Liste aller registrierten Benutzer.</returns>
        Task<List<User>> GetAllUsersAsync();

        /// Löscht einen Benutzer anhand der Benutzer-ID.
        /// <returns>Gibt True zurück, wenn der Benutzer erfolgreich gelöscht wurde, andernfalls False.</returns>
        Task<bool> DeleteUserAsync(int userId);

      
        /// Weist einem Benutzer eine neue Rolle zu.        
        /// <returns>Gibt True zurück, wenn die Rollenänderung erfolgreich war, andernfalls False.</returns>
        Task<bool> SetUserRoleAsync(int userId, string newRole);

       
        /// Aktualisiert die Benutzerdaten eines bestehenden Benutzers.       
        /// <returns>Gibt True zurück, wenn die Aktualisierung erfolgreich war, andernfalls False.</returns>
        Task<bool> UpdateUserAsync(UserUpdateDto updateDto);
    }
}
