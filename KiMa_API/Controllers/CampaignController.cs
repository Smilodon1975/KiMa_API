using KiMa_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KiMa_API.Controllers
{
    [ApiController]
    [Route("api/campaign")]
    [Authorize(Roles = "Admin")]
    public class CampaignController : ControllerBase
    {
        private readonly IEmailCampaignService _campaignService;

        public CampaignController(IEmailCampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        /// <summary>
        /// (No-Op bei ACS) Fügt einen Subscriber hinzu.
        /// </summary>
        [HttpPost("subscriber/add")]
        public async Task<IActionResult> AddSubscriber([FromBody] EmailDto dto)
        {
            await _campaignService.AddSubscriberAsync(dto.Email);
            return Ok(new { message = "Subscriber hinzugefügt." });
        }

        /// <summary>
        /// (No-Op bei ACS) Entfernt einen Subscriber.
        /// </summary>
        [HttpPost("subscriber/remove")]
        public async Task<IActionResult> RemoveSubscriber([FromBody] EmailDto dto)
        {
            await _campaignService.RemoveSubscriberAsync(dto.Email);
            return Ok(new { message = "Subscriber entfernt." });
        }

        /// <summary>
        /// Sendet eine Kampagne mit Anhang an alle angegebenen Empfänger.
        /// </summary>
        [HttpPost("send")]
        public async Task<IActionResult> SendCampaign([FromForm] CampaignDto dto)
        {
            if (dto.Attachment == null || dto.Recipients == null || dto.Recipients.Count == 0)
                return BadRequest("Anhang und Empfänger sind erforderlich.");

            await using var stream = dto.Attachment.OpenReadStream();
            await _campaignService.SendCampaignAsync(
                dto.CampaignName,
                stream,
                dto.Attachment.FileName,
                dto.Recipients
            );

            return Ok(new { message = "Kampagne versendet." });
        }
    }

    /// <summary>
    /// DTO für eine einzelne E-Mail-Adresse
    /// </summary>
    public class EmailDto
    {
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO für den Kampagnen-Versand
    /// </summary>
    public class CampaignDto
    {
        /// <summary>Name/Titel der Kampagne</summary>
        public string CampaignName { get; set; } = string.Empty;

        /// <summary>Hochgeladene Datei (z. B. PDF oder Word)</summary>
        public IFormFile Attachment { get; set; } = default!;

        /// <summary>Liste der Empfänger-E-Mail-Adressen</summary>
        public List<string> Recipients { get; set; } = new();
    }
}
