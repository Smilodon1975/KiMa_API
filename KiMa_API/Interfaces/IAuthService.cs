using KiMa_API.Models;

namespace KiMa_API.Services
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(RegisterModel model);
    }
}

