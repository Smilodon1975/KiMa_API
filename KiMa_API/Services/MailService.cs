using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace KiMa_API.Services
{
    public class MailService : IMailService
    {
        private readonly IConfiguration _config;

        public MailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetToken)
        {
            var smtpServer = _config["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_config["EmailSettings:SmtpPort"]);
            var smtpUsername = _config["EmailSettings:SmtpUsername"];
            var smtpPassword = _config["EmailSettings:SmtpPassword"];
            var fromEmail = _config["EmailSettings:FromEmail"];

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true
            };

            var resetLink = $"https://deine-app.com/reset-password?token={resetToken}";
            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = "Passwort zurücksetzen",
                Body = $"<p>Klicke auf den folgenden Link, um dein Passwort zurückzusetzen:</p> <a href='{resetLink}'>Passwort zurücksetzen</a>",
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            try
            {
                await client.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Mailversand fehlgeschlagen: {ex.Message}");
                return false;
            }
        }
    }
}
