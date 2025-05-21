using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
        public async Task<bool> SendWelcomeEmailAsync(string toEmail, string userName)
        {
            // Basis-URL aus Config
            var frontendUrl = _config["Frontend:BaseUrl"]
                             ?? throw new InvalidOperationException("Missing Frontend:BaseUrl");
            // Logo-Pfad
            var logoUrl = $"{frontendUrl}/assets/images/kima_logo_gruen_rot_rgb.png";
            var loginLink = $"{frontendUrl}/login";

            // HTML-Template
            var html = $@"
                <div style=""font-family:sans-serif;line-height:1.5;color:#333;"">
                  <p><img src=""{logoUrl}"" alt=""KiMa Logo"" style=""width:120px;"" /></p>
                  <h1>Hallo {WebUtility.HtmlEncode(userName)},</h1>
                  <p>willkommen bei <strong>KiMa</strong>! 🎉 Wir freuen uns sehr, dich in unserer Marktforschungs-Community begrüßen zu dürfen.</p>
                  <ul>
                    <li>👉 Nimm an spannenden Online-Studien teil</li>
                    <li>👉 Teile deine Meinung mit Unternehmen</li>
                    <li>👉 Verdiene dir ein nettes Zusatzeinkommen</li>
                  </ul>
                  <p>Falls du dich nicht selbst registriert hast, könnte jemand deine E-Mail-Adresse versehentlich verwendet haben. In diesem Fall ignoriere diese Nachricht bitte oder wende dich an unseren Support unter <a href=""mailto:info@kimafo.de"">info@kimafo.de</a>.</p>
                  <p style=""text-align:center;margin:20px 0;"">
                    <a href=""{loginLink}"" style=""display:inline-block;padding:12px 24px;
                       background-color:#0078D4;color:#fff;text-decoration:none;
                       border-radius:4px;"">
                      Jetzt einloggen
                    </a>
                  </p>
                  <hr style=""border:none;border-top:1px solid #eee;"" />
                  <p style=""font-size:0.85em;color:#666;"">
                    Dies ist eine automatisch generierte E-Mail – bitte antworte nicht darauf.<br/>
                    Bei Fragen erreichst du uns jederzeit unter <a href=""mailto:info@kimafo.de"">info@kimafo.de</a>.
                  </p>
                </div>";

            // Plain-Text-Fallback
            var plain = new StringBuilder();
            plain.AppendLine($"Hallo {userName},");
            plain.AppendLine();
            plain.AppendLine("Willkommen bei KiMa! Wir freuen uns, dich in unserer Marktforschungs-Community zu begrüßen.");
            plain.AppendLine("- Nimm an spannenden Online-Studien teil");
            plain.AppendLine("- Teile deine Meinung mit Unternehmen");
            plain.AppendLine("- Verdiene dir ein nettes Zusatzeinkommen");
            plain.AppendLine();
            plain.AppendLine("Falls du dich nicht selbst registriert hast, wurde möglicherweise deine E-Mail-Adresse versehentlich verwendet. Ignoriere diese Nachricht bitte oder kontaktiere unseren Support unter support@kimafo.info.");
            plain.AppendLine();
            plain.AppendLine($"Zum Einloggen klicke hier: {loginLink}");
            plain.AppendLine();
            plain.AppendLine("Dies ist eine automatisch generierte E-Mail – bitte antworte nicht darauf.");
            plain.AppendLine("Bei Fragen erreichst du uns unter info@kimafo.de.");

            var content = new EmailContent("Willkommen bei KiMa")
            {
                PlainText = plain.ToString(),
                Html = html
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

    }
}
