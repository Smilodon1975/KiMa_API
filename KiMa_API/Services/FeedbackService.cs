using KiMa_API.Data;
using KiMa_API.Models;
using KiMa_API.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace KiMa_API.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly AppDbContext _ctx;
        private readonly IMailService _mail;

        public FeedbackService(AppDbContext ctx, IMailService mail)
        {
            _ctx = ctx;
            _mail = mail;
        }

        public async Task SubmitAsync(FeedbackDto dto)
        {
            var fb = new Feedback
            {
                UserId = dto.UserId,
                Email = dto.Email,
                Content = dto.Content
            };
            _ctx.Feedbacks.Add(fb);
            await _ctx.SaveChangesAsync();

            // E-Mail an Admin
            var subject = "Neues Test-Feedback von KiMa";
            var body = $"<p><strong>Von:</strong> {dto.Email ?? "Gast"}</p>" +
                       $"<p><strong>Inhalt:</strong><br/>{dto.Content}</p>";
            await SendEmailAsync("admin@kimafo.info", subject, body);
        }

        private async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            // Assuming IMailService does not have a generic SendEmailAsync method,
            // you can implement a helper method to use one of the existing methods.
            return await _mail.SendEmailConfirmationEmailAsync(toEmail, subject, body);
        }

        public async Task<List<Feedback>> GetAllAsync()
            => await _ctx.Feedbacks.OrderByDescending(f => f.CreatedAt).ToListAsync();

        public async Task<bool> DeleteAsync(int id)
        {
            var fb = await _ctx.Feedbacks.FindAsync(id);
            if (fb == null) return false;
            _ctx.Feedbacks.Remove(fb);
            await _ctx.SaveChangesAsync();
            return true;
        }
    }
}

