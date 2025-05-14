namespace KiMa_API.Services
{
    public interface IEmailCampaignService
    {
        Task AddSubscriberAsync(string email);
        Task RemoveSubscriberAsync(string email);
        Task SendCampaignAsync(
            string campaignName,
            Stream attachmentStream,
            string fileName,
            IEnumerable<string> recipientEmails
        );
    }

}

