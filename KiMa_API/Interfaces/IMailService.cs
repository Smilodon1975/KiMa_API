using System.Threading.Tasks;

namespace KiMa_API.Services
{
    public interface IMailService
    {
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetToken);
    }
}

