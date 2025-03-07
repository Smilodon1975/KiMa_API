using System.Threading.Tasks;

namespace KiMa_API.Services
{
 
    /// Schnittstelle für den E-Mail-Service, der das Versenden von Passwort-Reset-E-Mails übernimmt.
  
    public interface IMailService
    {
     
        /// Sendet eine E-Mail mit einem Passwort-Reset-Token an den Benutzer.
        /// <returns>True, wenn die E-Mail erfolgreich versendet wurde, andernfalls False.</returns>
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetToken);
    }
}
