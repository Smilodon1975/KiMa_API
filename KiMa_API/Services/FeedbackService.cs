using KiMa_API.Data;
using KiMa_API.Models;
using KiMa_API.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System.Net;

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

        //public async Task SubmitAsync(FeedbackDto dto)
        //{
        //    string? email = dto.Email;

        //    if (dto.UserId.HasValue)
        //    {
        //        var user = await _ctx.Users.FindAsync(dto.UserId.Value);
        //        if (user is not null)
        //            email = user.Email;
        //    }

        //    var fb = new Feedback
        //    {
        //        UserId = dto.UserId,
        //        Email = email,
        //        Content = dto.Content
        //    };
        //    _ctx.Feedbacks.Add(fb);
        //    await _ctx.SaveChangesAsync();

        //    var adminEmail = "techsupport@kimafo.de";
        //    var subject = "Neues Feedback bei KiMa";
        //    var htmlBody = $@"
        //    <p><strong>Von:</strong> {WebUtility.HtmlEncode(dto.Email ?? "Gast")}</p>
        //    <p><strong>Inhalt:</strong><br/>{WebUtility.HtmlEncode(dto.Content)}</p>
        //";
        //    var textBody = $"Von: {dto.Email ?? "Gast"}\nInhalt: {dto.Content}";

        //    await _mail.SendNotificationEmailAsync( adminEmail, subject, htmlBody,textBody);
        //}

        public async Task SubmitAsync(FeedbackDto dto)
        {
            // 1️⃣ echten Absender ermitteln
            string senderEmail = dto.Email ?? "Gast";
            string senderName = "Gast";

            if (dto.UserId.HasValue)
            {
                var user = await _ctx.Users.FindAsync(dto.UserId.Value);
                if (user is not null)
                {
                    senderEmail = user.Email!;
                    // wenn du zusätzlich den Username zeigen willst:
                    senderName = string.IsNullOrWhiteSpace(user.UserName)
                        ? senderEmail
                        : user.UserName;
                }
            }

            // 2️⃣ Feedback in DB speichern
            var fb = new Feedback
            {
                UserId = dto.UserId,
                Email = senderEmail,
                Content = dto.Content
            };
            _ctx.Feedbacks.Add(fb);
            await _ctx.SaveChangesAsync();

            // 3️⃣ Mail an Admin zusammenbauen
            var subject = "Neues Feedback bei KiMa";
            var body =
                $"<p><strong>Von:</strong> {WebUtility.HtmlEncode(senderName)} &lt;{WebUtility.HtmlEncode(senderEmail)}&gt;</p>" +
                $"<p><strong>Inhalt:</strong><br/>{WebUtility.HtmlEncode(dto.Content).Replace("\n", "<br/>")}</p>";

            // 4️⃣ Mail verschicken (hier nutzt du deinen MailService)
            await _mail.SendNotificationEmailAsync(
                toEmail: "techsupport@kimafo.de",
                subject: subject,
                htmlContent: body
            );
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

