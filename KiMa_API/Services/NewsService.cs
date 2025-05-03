using KiMa_API.Data;
using KiMa_API.Models;
using Microsoft.EntityFrameworkCore;

namespace KiMa_API.Services
{
    public class NewsService : INewsService
    {
        private readonly AppDbContext _context;

        public NewsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<News>> GetAllNewsAsync()
        {
            return await _context.News.OrderByDescending(n => n.PublishDate).ToListAsync();
        }

        public async Task<News> GetNewsByIdAsync(int id)
        {
            return await _context.News.FindAsync(id);
        }

        public async Task<News> CreateNewsAsync(News news)
        {
            _context.News.Add(news);
            await _context.SaveChangesAsync();
            return news;
        }

        public async Task<bool> UpdateNewsAsync(News news)
        {
            _context.News.Update(news);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteNewsAsync(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null) return false;
            _context.News.Remove(news);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
