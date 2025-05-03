using KiMa_API.Data;
using KiMa_API.Models;
using KiMa_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KiMa_API.Controllers
{
    [ApiController]
    [Route("api/newsletter")]
    public class NewsletterController : ControllerBase
    {
        private readonly AppDbContext _ctx;
        private readonly IRapidmailService _rm;
        public NewsletterController(AppDbContext ctx, IRapidmailService rm)
        {
            _ctx = ctx; _rm = rm;
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] string email)
        {
            // 1) In eigener DB speichern (oder updaten)
            var sub = await _ctx.NewsletterSubscribers
                        .FirstOrDefaultAsync(s => s.Email == email)
                      ?? new NewsletterSubscriber { Email = email };
            sub.IsSubscribed = true;
            if (sub.Id == 0) _ctx.Add(sub);
            await _ctx.SaveChangesAsync();

            // 2) Parallel bei Rapidmail anlegen
            await _rm.AddSubscriberAsync(email);

            return Ok();
        }

        [HttpPost("unsubscribe")]
        public async Task<IActionResult> Unsubscribe([FromBody] string email)
        {
            var sub = await _ctx.NewsletterSubscribers
                        .FirstOrDefaultAsync(s => s.Email == email);
            if (sub is not null)
            {
                sub.IsSubscribed = false;
                await _ctx.SaveChangesAsync();
            }
            await _rm.RemoveSubscriberAsync(email);
            return Ok();
        }
    }
}
