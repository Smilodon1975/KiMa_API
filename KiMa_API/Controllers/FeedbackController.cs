using KiMa_API.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KiMa_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedback;

        public FeedbackController(IFeedbackService feedback) => _feedback = feedback;


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FeedbackDto dto)
        {
            // Ensure the FeedbackDto being passed matches the expected type in IFeedbackService
            await _feedback.SubmitAsync(dto);
            return Ok(new { message = "Danke für dein Feedback!" });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Get() =>
            Ok(await _feedback.GetAllAsync());

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) =>
            await _feedback.DeleteAsync(id)
              ? Ok()
              : NotFound();
    }

}
