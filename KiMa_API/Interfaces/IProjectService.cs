using KiMa_API.Models;
using KiMa_API.Models.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KiMa_API.Services
{
    public interface IProjectService
    {
        Task<Project> CreateAsync(Project project);
        Task<Project?> GetByIdAsync(int id);
        Task<IEnumerable<Project>> GetAllAsync();
        Task<bool> UpdateAsync(Project project);
        Task<bool> DeleteAsync(int id);
        
    }
}
