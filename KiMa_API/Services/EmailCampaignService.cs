using Azure.Communication.Email;

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

       
        // Sendet eine Kampagne mit Anhang an die Empfänger.      
        public async Task SendCampaignAsync(
    string campaignName,
    Stream attachmentStream,
    string fileName,
    IEnumerable<string> recipientEmails
)
        {
            // E-Mail-Inhalt
            var content = new EmailContent(campaignName)
            {
                PlainText = "Bitte finden Sie die angehängte Projektinformation.",
                Html = "<p>Bitte finden Sie im Anhang die Projektinformation als PDF.</p>"
            };

            // Anhang erstellen
            var bytes = ReadAllBytes(attachmentStream);
            var attachment = new EmailAttachment(
                name: fileName,
                content: BinaryData.FromBytes(bytes),
                contentType: "application/pdf"
            );

            // Empfängerliste erzeugen
            var toAddresses = recipientEmails
                .Select(addr => new EmailAddress(addr))
                .ToList();
            var recipients = new EmailRecipients(toAddresses);

            // Nachricht erstellen
            var message = new EmailMessage(_fromAddress, recipients, content)
            {
                Attachments = { attachment } // Attachments are added here
            };

            // Replace the problematic line with the following:
            var response = await _emailClient.SendAsync(Azure.WaitUntil.Completed, message);
            if (response.Value.Status != EmailSendStatus.Succeeded)
            {
                throw new InvalidOperationException($"E-Mail-Versand fehlgeschlagen: {response.Value.Status}");
            }
        }

        private static byte[] ReadAllBytes(Stream stream)
        {
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }
}
