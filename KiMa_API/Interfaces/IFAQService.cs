using KiMa_API.Models;

namespace KiMa_API.Services
{
    public interface IFAQService
    {
        Task<IEnumerable<FAQ>> GetAllFAQsAsync();
        Task<FAQ> GetFAQByIdAsync(int id);
        Task<FAQ> CreateFAQAsync(FAQ faq);
        Task<bool> UpdateFAQAsync(FAQ faq);
        Task<bool> DeleteFAQAsync(int id);
    }
}
