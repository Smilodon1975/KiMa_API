using KiMa_API.Data;
using KiMa_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KiMa_API.Services
{
    public class FAQService
    {
        private readonly AppDbContext _context;

        public FAQService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FAQ>> GetAllFAQsAsync()
        {
            return await _context.FAQs.OrderBy(f => f.Order).ToListAsync();
        }

        public async Task<FAQ> GetFAQByIdAsync(int id)
        {
            return await _context.FAQs.FindAsync(id);
        }

        public async Task<FAQ> CreateFAQAsync(FAQ faq)
        {
            _context.FAQs.Add(faq);
            await _context.SaveChangesAsync();
            return faq;
        }

        public async Task<bool> UpdateFAQAsync(FAQ faq)
        {
            _context.FAQs.Update(faq);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteFAQAsync(int id)
        {
            var faq = await _context.FAQs.FindAsync(id);
            if (faq == null) return false;
            _context.FAQs.Remove(faq);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

