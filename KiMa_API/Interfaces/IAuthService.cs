using KiMa_API.Models;
using KiMa_API.Models.Dto;
using Microsoft.AspNetCore.Identity;

namespace KiMa_API.Services
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(string email, string password);      
        Task<IdentityResult> RegisterAsync(RegisterModel model);        
        Task<string> GeneratePasswordResetTokenAsync(string email);
        Task<bool> ResetPasswordAsync(PasswordResetDto model);
    }
}

