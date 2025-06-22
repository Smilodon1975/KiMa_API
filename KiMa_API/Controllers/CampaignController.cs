using KiMa_API.Data;
using KiMa_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KiMa_API.Controllers
{
    [ApiController]
    [Route("api/campaign")]
    [Authorize(Roles = "Admin")]
    public class CampaignController : ControllerBase
    {
        private readonly IEmailCampaignService _campaignService;
        private readonly AppDbContext _dbContext;

        public CampaignController(IEmailCampaignService campaignService, AppDbContext dbContext)
        {
            _campaignService = campaignService;
            _dbContext = dbContext;
        }


        //====================================================================================================
        //User für Versand abrufen

        [HttpGet("campaignUsers")]
        public async Task<PaginatedResult<CampaignUser>> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _dbContext.Users
                .Select(u => new CampaignUser
                {
                    Id = u.Id,
                    Email = u.Email ?? string.Empty,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    NewsletterSub = u.NewsletterSub
                });

            var total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<CampaignUser>
            {
                Items = items,
                TotalCount = total
            };
        }


        //====================================================================================================
        //Email versenden

        [HttpPost("send")]
        public async Task<IActionResult> SendCampaign([FromForm] CampaignDto dto, IFormFile? attachment)
        {
            if (dto.Recipients == null || dto.Recipients.Count == 0)
                return BadRequest("Empfänger sind erforderlich.");

            Stream? stream = null;
            string? filename = null;
            if (attachment != null && attachment.Length > 0)
            {
                filename = Path.GetFileName(attachment.FileName);
                stream = attachment.OpenReadStream();
            }

            await _campaignService.SendCampaignAsync(
                dto.CampaignName,
                dto.Subject,
                dto.Body,
                dto.Link,
                dto.Recipients,
                stream,
                filename
            );

            return Ok(new { message = "Kampagne versendet." });
        }
    }


    //====================================================================================================
            //DTO´s
            public class EmailDto
            {
                public string Email { get; set; } = string.Empty;
            }

   
            public class CampaignDto
            {
                public string CampaignName { get; set; } = string.Empty;
                public List<string> Recipients { get; set; } = new();
                public string Subject { get; set; } = string.Empty;
                public string Body { get; set; } = string.Empty;
                public string Link { get; set; } = string.Empty;
    }

            public class PaginatedResult<T>
            {
                public List<T> Items { get; set; } = new();
                public int TotalCount { get; set; }
            }

            public class CampaignUser
            {
                public int Id { get; set; }
                public string Email { get; set; } = string.Empty;
                public string? UserName { get; set; }
                public string? FirstName { get; set; }
                public string? LastName { get; set; }
                public bool NewsletterSub { get; set; }
            }
}
