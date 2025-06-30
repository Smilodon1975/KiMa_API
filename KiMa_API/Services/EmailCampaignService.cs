using Azure;
using Azure.Communication.Email;
using System.Net;

namespace KiMa_API.Services
{
    public class EmailCampaignService : IEmailCampaignService
    {
        private readonly EmailClient _emailClient;
        private readonly string _fromAddress;

        public EmailCampaignService(EmailClient emailClient, IConfiguration config)
        {
            _emailClient = emailClient;
            _fromAddress = config["EmailSettings:FromEmail"]
                ?? throw new InvalidOperationException("Missing EmailSettings:FromEmail");
        }

        // Subscribe/Unsubscribe sind bei ACS nicht nötig, daher No-Op.        
        public Task AddSubscriberAsync(string email) => Task.CompletedTask;
        public Task RemoveSubscriberAsync(string email) => Task.CompletedTask;


        public async Task SendCampaignAsync(string campaignName, string subject, string body, string link,
            IEnumerable<string> recipientEmails, Stream? attachmentStream, string? attachmentFileName)
        {
            // E-Mail-Inhalt
            var htmlBody = $@"
              <html>
                <body>
                 
                  <h1 style=""font-family:Arial,sans-serif;color:#333;margin-bottom:1rem;"">
                    {WebUtility.HtmlEncode(campaignName)}
                  </h1>

                  
                  <div style=""font-family:Arial,sans-serif;color:#555;margin-bottom:1.5rem;"">
                    {WebUtility.HtmlEncode(body).Replace("\n", "<br/>")}
                  </div>";

                  
                    if (!string.IsNullOrWhiteSpace(link))
                    {
                        htmlBody += $@"
                  <p>
                    <a href=""{link}"" 
                       style=""display:inline-block;padding:0.5rem 1rem;
                              background:#16A085;color:#fff;
                              text-decoration:none;border-radius:4px;"">
                      {WebUtility.HtmlEncode(campaignName)}
                    </a>
                  </p>";
                    }
                    htmlBody += @"
                </body>
              </html>";

            var content = new EmailContent(subject)
            {
                PlainText = body,
                Html = htmlBody
            };

            // Attachments are not directly supported by EmailContent. Instead, use EmailMessage.Attachments.
            var attachments = new List<EmailAttachment>();
            if (attachmentStream is not null && !string.IsNullOrEmpty(attachmentFileName))
            {
                using var ms = new MemoryStream();
                await attachmentStream.CopyToAsync(ms);

                attachments.Add(new EmailAttachment(
                    attachmentFileName,
                    "application/octet-stream",
                    BinaryData.FromBytes(ms.ToArray())
                ));
            }

            // Empfängerliste
            var to = recipientEmails.Select(email => new EmailAddress(email)).ToList();
            var recipients = new EmailRecipients(to);

            // Nachricht verschicken
            var message = new EmailMessage(_fromAddress, recipients, content);

            // Fix for CS0200: Attachments property is read-only. Use Add method instead.
            foreach (var attachment in attachments)
            {
                message.Attachments.Add(attachment);
            }

            var response = await _emailClient.SendAsync(WaitUntil.Completed, message);

            if (response.Value.Status != EmailSendStatus.Succeeded)
                throw new InvalidOperationException(
                    $"E-Mail Versand fehlgeschlagen: {response.Value.Status}"
                );
        }


        public async Task SendNotificationAsync(string to, string subject, string plainTextBody, string htmlBody)
                {
                    var content = new EmailContent(subject)
                    {
                        PlainText = plainTextBody,
                        Html = htmlBody
                    };

                    var recipients = new EmailRecipients(new[] { new EmailAddress(to) });
                    var message = new EmailMessage(_fromAddress, recipients, content);

                    await _emailClient.SendAsync(WaitUntil.Completed, message);
                }



        
    }
}
