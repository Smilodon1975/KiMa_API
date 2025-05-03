using KiMa_API.Models;
using KiMa_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace KiMa_API.Controllers
{
    [ApiController]
    [Route("api/faq")]
    [Authorize(Roles = "Admin")] // Nur Admins dürfen FAQ bearbeiten
    public class FAQController : ControllerBase
    {
        private readonly IFAQService _faqService;

        public FAQController(IFAQService faqService)
        {
            _faqService = faqService;
        }

        [HttpGet]
        [AllowAnonymous] // Falls auch Nicht-Admins die FAQ sehen sollen
        public async Task<ActionResult<IEnumerable<FAQ>>> GetFAQs()
        {
            var faqs = await _faqService.GetAllFAQsAsync();
            return Ok(faqs);
        }

        [HttpPost]
        public async Task<ActionResult<FAQ>> CreateFAQ([FromBody] FAQ faq)
        {
            var createdFAQ = await _faqService.CreateFAQAsync(faq);
            return CreatedAtAction(nameof(GetFAQById), new { id = createdFAQ.Id }, createdFAQ);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FAQ>> GetFAQById(int id)
        {
            var faq = await _faqService.GetFAQByIdAsync(id);
            if (faq == null) return NotFound();
            return Ok(faq);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFAQ(int id, [FromBody] FAQ faq)
        {
            if (id != faq.Id) return BadRequest();
            var success = await _faqService.UpdateFAQAsync(faq);
            if (!success) return BadRequest("Update fehlgeschlagen.");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFAQ(int id)
        {
            var success = await _faqService.DeleteFAQAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}

