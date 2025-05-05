using System.Net;
using System.Net.Mail;

namespace KiMa_API.Services
{
    // Service zum Versenden von E-Mails, speziell für Passwort-Reset-Anfragen.
    public class MailService : IMailService
    {
        private readonly IConfiguration _config;

        public MailService(IConfiguration config)
        {
            _config = config;
        }

        // Sendet eine E-Mail mit einem Passwort-Reset-Link an den angegebenen Empfänger.
        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetToken, string userName)
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

            var frontendUrl = "https://kimafo.info/reset-password";
            var resetLink = $"{frontendUrl}?token={WebUtility.UrlEncode(resetToken)}" +
                            $"&email={WebUtility.UrlEncode(toEmail)}" +
                            $"&userName={WebUtility.UrlEncode(userName)}";

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
                Console.WriteLine($"[INFO] Passwort-Reset-Mail an {toEmail} gesendet.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Mailversand fehlgeschlagen: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> SendEmailConfirmationEmailAsync(string toEmail, string confirmationToken, string userName)
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

            // Setze hier die URL deines Frontends, das die Bestätigung verarbeitet
            var frontendUrl = "https://kimafo.info/confirm-email";
            var confirmationLink = $"{frontendUrl}?token={WebUtility.UrlEncode(confirmationToken)}" +
                                   $"&email={WebUtility.UrlEncode(toEmail)}" +
                                   $"&userName={WebUtility.UrlEncode(userName)}";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = "E-Mail Bestätigung",
                Body = $"<p>Hallo {userName},</p>" +
                       $"<p>Bitte bestätige deine E-Mail-Adresse, indem du auf den folgenden Link klickst:</p>" +
                       $"<a href='{confirmationLink}'>E-Mail bestätigen</a>",
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            try
            {
                await client.SendMailAsync(mailMessage);
                Console.WriteLine($"[INFO] Bestätigungs-Mail an {toEmail} gesendet.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Bestätigungs-Mail Versand fehlgeschlagen: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> SendPasswordChangedNotificationEmailAsync(string toEmail, string userName)
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

            var subject = "Dein Passwort wurde geändert";
            var body = $"<p>Hallo {userName},</p>" +
                       "<p>Dies ist eine Bestätigung, dass dein Passwort erfolgreich geändert wurde.</p>" +
                       "<p>Falls du diese Änderung nicht veranlasst hast, kontaktiere bitte umgehend unseren Support.</p>" +
                       "<p>Viele Grüße,<br/>Dein Team</p>";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            try
            {
                await client.SendMailAsync(mailMessage);
                Console.WriteLine($"[INFO] Passwort-Änderungs-Benachrichtigung an {toEmail} gesendet.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Passwort-Änderungs-Benachrichtigung konnte nicht gesendet werden: {ex.Message}");
                return false;
            }
        }

        

    }
}

