using KiMa_API.Interfaces;
using KiMa_API.Models;
using KiMa_API.Models.Dto;
using KiMa_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace KiMa_API.Controllers
{
    [ApiController]
    [Route("api/projectResponses")]
    public class ProjectResponsesController : ControllerBase
    {
        private readonly IProjectResponseService _responseService;
        private readonly IProjectService _projectService;
        private readonly IEmailCampaignService _emailCampaignService;
        private readonly string _adminEmail;

        public ProjectResponsesController(IProjectResponseService responseService, IProjectService projectService,
            IEmailCampaignService emailCampaignService, IConfiguration config)
        {
            _responseService = responseService;
            _projectService = projectService;
            _emailCampaignService = emailCampaignService;
            _adminEmail = config["Notification:AdminEmail"]
                          ?? throw new InvalidOperationException("AdminEmail fehlt in Konfiguration");
        }



        [HttpPost("{projectId}/responses")]
        public async Task<ActionResult<ProjectResponse>> SubmitResponse(
            int projectId, [FromBody] ProjectResponseDto dto)
        {
            if (projectId != dto.ProjectId)
                return BadRequest("Projekt-ID stimmt nicht überein.");

            var created = await _responseService.SubmitResponseAsync(dto);

            var project = await _projectService.GetByIdAsync(projectId);
            var projName = project?.Name ?? $"#{projectId}";

            // 3) Notification-Mail vorbereiten
            var subject = $"Neue Antwort zu Projekt „{projName}“";
            var body = $"Der User mit der E-Mailadresse „{dto.RespondentEmail}“ " +
                          $"hat eine Antwort zum Projekt „{projName}“ abgegeben.";

            // 4) Mail senden (fire-and-forget oder await)
            await _emailCampaignService .SendNotificationAsync(_adminEmail, subject, body);

            return CreatedAtAction(nameof(GetResponses), new { projectId = projectId }, created);
        }



        [HttpGet("{projectId}/responses")]
        public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetResponses(int projectId)
        {
            var list = await _responseService.GetResponsesForProjectAsync(projectId);
            return Ok(list);
        }


        [HttpGet("{projectId}/responses/hasResponded")]
        public async Task<ActionResult<bool>> HasResponded(
        int projectId,
        [FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("E-Mail darf nicht leer sein.");

            var exists = await _responseService.HasRespondedAsync(projectId, email);
            return Ok(exists);
        }

    }
}
