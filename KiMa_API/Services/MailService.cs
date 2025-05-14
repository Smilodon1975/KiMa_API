using System;
using System.Threading.Tasks;
using System.Net;
using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Configuration;

namespace KiMa_API.Services
{
    /// <summary>
    /// Service zum Versenden von E-Mails über Azure Communication Services.
    /// </summary>
    public class MailService : IMailService
    {
        private readonly EmailClient _emailClient;
        private readonly IConfiguration _config;
        private readonly string _fromAddress;
        private readonly string _frontendBaseUrl;

        public MailService(EmailClient emailClient, IConfiguration config)
        {
            _emailClient = emailClient ?? throw new ArgumentNullException(nameof(emailClient));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _fromAddress = _config["EmailSettings:FromEmail"]
                ?? throw new InvalidOperationException("Missing EmailSettings:FromEmail");
            _frontendBaseUrl = _config["AppSettings:FrontendBaseUrl"] ?? "https://kimafo.info";
        }

        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetToken, string userName)
        {
            var resetLink = $"{_frontendBaseUrl}/reset-password?token={WebUtility.UrlEncode(resetToken)}"
                            + $"&email={WebUtility.UrlEncode(toEmail)}"
                            + $"&userName={WebUtility.UrlEncode(userName)}";

            var content = new EmailContent("Passwort zurücksetzen")
            {
                PlainText = $"Hallo {userName},\nBitte klicke auf den Link, um dein Passwort zurückzusetzen: {resetLink}",
                Html = $"<p>Hallo {WebUtility.HtmlEncode(userName)},</p>"
                     + $"<p>Bitte klicke auf den folgenden Link, um dein Passwort zurückzusetzen:</p>"
                     + $"<p><a href='{resetLink}'>Passwort zurücksetzen</a></p>"
            };

            return await SendAsync(toEmail, content);
        }

        public async Task<bool> SendEmailConfirmationEmailAsync(string toEmail, string confirmationToken, string userName)
        {
            var confirmationLink = $"{_frontendBaseUrl}/confirm-email?token={WebUtility.UrlEncode(confirmationToken)}"
                                 + $"&email={WebUtility.UrlEncode(toEmail)}"
                                 + $"&userName={WebUtility.UrlEncode(userName)}";

            var content = new EmailContent("E-Mail Bestätigung")
            {
                PlainText = $"Hallo {userName},\nBitte bestätige deine E-Mail-Adresse, indem du auf den folgenden Link klickst: {confirmationLink}",
                Html = $"<p>Hallo {WebUtility.HtmlEncode(userName)},</p>"
                     + $"<p>Bitte bestätige deine E-Mail-Adresse, indem du auf den folgenden Link klickst:</p>"
                     + $"<p><a href='{confirmationLink}'>E-Mail bestätigen</a></p>"
            };

            return await SendAsync(toEmail, content);
        }

        public async Task<bool> SendPasswordChangedNotificationEmailAsync(string toEmail, string userName)
        {
            var subject = "Dein Passwort wurde geändert";
            var htmlBody = $"<p>Hallo {WebUtility.HtmlEncode(userName)},</p>"
                         + "<p>Dies ist eine Bestätigung, dass dein Passwort erfolgreich geändert wurde.</p>"
                         + "<p>Falls du diese Änderung nicht veranlasst hast, kontaktiere bitte umgehend unseren Support.</p>"
                         + "<p>Viele Grüße,<br/>Dein Team</p>";

            var content = new EmailContent(subject)
            {
                PlainText = $"Hallo {userName},\nDies ist eine Bestätigung, dass dein Passwort erfolgreich geändert wurde.\nViele Grüße, Dein Team",
                Html = htmlBody
            };

            return await SendAsync(toEmail, content);
        }

        private async Task<bool> SendAsync(string toEmail, EmailContent content)
        {
            var recipients = new EmailRecipients(new[] { new EmailAddress(toEmail) });
            var message = new EmailMessage(_fromAddress, recipients, content);

            try
            {
                var response = await _emailClient.SendAsync(WaitUntil.Completed, message);
                return response.Value.Status == EmailSendStatus.Succeeded;
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine($"[ERROR] E-Mail-Versand fehlgeschlagen: {ex.Message}");
                return false;
            }
        }
    }
}
