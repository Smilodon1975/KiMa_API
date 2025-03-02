using KiMa_API.Models;
using KiMa_API.Models.Dto;
using System.Threading.Tasks;
using static KiMa_API.Controllers.UserController;

namespace KiMa_API.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetMyDataAsync(int requestUserId);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<bool> UpdateUserAsync(UserUpdateModel userUpdate, int requestUserId, string requestUserRole);
    }
}

