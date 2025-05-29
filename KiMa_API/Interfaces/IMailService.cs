namespace KiMa_API.Services
{

    /// Schnittstelle für den E-Mail-Service, der das Versenden von Passwort-Reset-E-Mails übernimmt.

    public interface IMailService
    {

        /// Sendet eine E-Mail mit einem Passwort-Reset-Token an den Benutzer.
        /// <returns>True, wenn die E-Mail erfolgreich versendet wurde, andernfalls False.</returns>
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetToken, string userName);
        Task<bool> SendEmailConfirmationEmailAsync(string toEmail, string confirmationToken, string userName);
        Task<bool> SendPasswordChangedNotificationEmailAsync(string toEmail, string userName);
        Task<bool> SendWelcomeEmailAsync(string toEmail, string userName);
        Task<bool> SendNotificationEmailAsync(string toEmail, string subject, string htmlContent, string plainTextContent = null);

    }
}
