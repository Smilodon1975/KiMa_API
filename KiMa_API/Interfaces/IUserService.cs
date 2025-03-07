using KiMa_API.Models;
using KiMa_API.Models.Dto;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace KiMa_API.Services
{

    /// Schnittstelle für Benutzerverwaltungsfunktionen, einschließlich Datenabruf und Aktualisierung.
   
    public interface IUserService
    {
      
        /// Ruft einen Benutzer anhand der ID ab.
        /// <returns>Ein Benutzerobjekt oder null, falls kein Benutzer gefunden wurde.</returns>
        Task<User?> GetUserByIdAsync(int id);

      
        /// Ruft die eigenen Benutzerdaten basierend auf der Anfrage-ID ab.
        /// <returns>Ein Benutzerobjekt oder null, falls der Benutzer nicht existiert.</returns>
        Task<User?> GetMyDataAsync(int requestUserId);

     
        /// Ruft eine Liste aller registrierten Benutzer ab.
        /// <returns>Eine Liste aller Benutzer.</returns>
        Task<IEnumerable<User>> GetAllUsersAsync();

      
        /// Aktualisiert die Benutzerdaten eines bestehenden Benutzers.
        /// <returns>True, wenn die Aktualisierung erfolgreich war, andernfalls False.</returns>
        Task<bool> UpdateUserAsync(UserUpdateModel userUpdate, int requestUserId, string requestUserRole);
    }
}
