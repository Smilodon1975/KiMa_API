using KiMa_API.Models;

namespace KiMa_API.Services
{
    public interface IAdminService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<bool> DeleteUserAsync(int userId);
        Task<bool> SetUserRoleAsync(int userId, string newRole);
    }
}

