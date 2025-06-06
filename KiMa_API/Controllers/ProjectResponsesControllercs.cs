using KiMa_API.Interfaces;
using KiMa_API.Models;
using KiMa_API.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace KiMa_API.Controllers
{
    [ApiController]
    [Route("api/projectResponses")]
    public class ProjectResponsesController : ControllerBase
    {
        private readonly IProjectResponseService _responseService;

        public ProjectResponsesController(IProjectResponseService responseService)
        {
            _responseService = responseService;
        }

        [HttpPost("{projectId}/responses")]
        public async Task<ActionResult<ProjectResponse>> SubmitResponse(
            int projectId,
            [FromBody] ProjectResponseDto dto)
        {
            if (projectId != dto.ProjectId)
                return BadRequest("Projekt-ID stimmt nicht überein.");

            var created = await _responseService.SubmitResponseAsync(dto);
            return CreatedAtAction(nameof(GetResponses), new { projectId = projectId }, created);
        }

        [HttpGet("{projectId}/responses")]
        public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetResponses(int projectId)
        {
            var list = await _responseService.GetResponsesForProjectAsync(projectId);
            return Ok(list);
        }
    }
}
