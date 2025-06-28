namespace KiMa_API.Services
{
    public interface IEmailCampaignService
    {
        Task AddSubscriberAsync(string email);
        Task RemoveSubscriberAsync(string email);
        Task SendCampaignAsync(string campaignName, string subject, string body,
        string link, IEnumerable<string> recipientEmails, Stream? attachmentStream,
        string? attachmentFileName
           );
        Task SendNotificationAsync(string to, string subject, string body);

    }

}

