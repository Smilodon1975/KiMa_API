using KiMa_API.Models;
using KiMa_API.Models.Dto;

namespace KiMa_API.Services
{
    public interface IAdminService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<bool> DeleteUserAsync(int userId);
        Task<bool> SetUserRoleAsync(int userId, string newRole);
        Task<bool> UpdateUserAsync(UserUpdateModel userUpdate );
    }
}

