using KiMa_API.Interfaces;
using KiMa_API.Models;
using KiMa_API.Models.Dto;
using KiMa_API.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

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
        public async Task<ActionResult<ProjectResponse>> SubmitResponse(int projectId, [FromBody] ProjectResponseDto dto)
        {
            if (projectId != dto.ProjectId)
                return BadRequest("Projekt-ID stimmt nicht überein.");

            // 1) Speichern
            var created = await _responseService.SubmitResponseAsync(dto);

            // 2) Projekt & Fragen laden
            var project = await _projectService.GetByIdAsync(projectId)
                          ?? throw new InvalidOperationException($"Projekt {projectId} nicht gefunden");
            var projName = WebUtility.HtmlEncode(project.Name);

            // 3) Fragen-Definitionen parsen
            var questionDefs = JsonConvert
                .DeserializeObject<List<QuestionDefinitionDto>>(project.QuestionsJson ?? "[]")
                ?? new List<QuestionDefinitionDto>();

            // 4) Antworten parsen
            var answersJ = JArray.Parse(dto.AnswersJson ?? "[]");
            var answers = answersJ.Select(x => new {
                QuestionId = (int)x["questionId"]!,
                // Wenn x["answer"] ein Array ist -> join, sonst ToString()
                Answer = x["answer"]!.Type == JTokenType.Array
                    ? string.Join(", ", x["answer"]!.ToObject<List<string>>()!)
                    : x["answer"]!.ToString()
            }).ToList();

            // Baue dann Deine Tabelle:
            var sb = new StringBuilder();
            sb.AppendLine(
                $"<p>Der User mit der E-Mailadresse „{WebUtility.HtmlEncode(dto.RespondentEmail)}“ " +
                $"hat eine Antwort zum Projekt „{WebUtility.HtmlEncode(projName)}“ abgegeben.</p>");
            sb.AppendLine("<table border=\"1\" cellpadding=\"5\" style=\"border-collapse:collapse\">");
            sb.AppendLine("<thead><tr><th>Frage</th><th>Antwort</th></tr></thead><tbody>");
            foreach (var ans in answers)
            {
                var qDef = questionDefs.FirstOrDefault(q => q.Id == ans.QuestionId);
                var qText = qDef != null
                    ? WebUtility.HtmlEncode(qDef.Text)
                    : $"Frage #{ans.QuestionId}";
                sb.AppendLine($"<tr><td>{qText}</td><td>{WebUtility.HtmlEncode(ans.Answer)}</td></tr>");
            }
            sb.AppendLine("</tbody></table>");

            // Mail senden
            await _emailCampaignService.SendNotificationAsync(
                _adminEmail,
                $"Neue Antwort zu „{WebUtility.HtmlEncode(project.Name)}“",
                /* plainText */ sb.ToString(),
                /* html */ sb.ToString());

            return CreatedAtAction(nameof(GetResponses), new { projectId }, created);
        }





        [HttpGet("{projectId}/responses")]
        public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetResponses(int projectId)
        {
            var list = await _responseService.GetResponsesForProjectAsync(projectId);
            return Ok(list);
        }


        [HttpGet("{projectId}/responses/hasResponded")]
        public async Task<ActionResult<bool>> HasResponded(int projectId, [FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("E-Mail darf nicht leer sein.");

            var exists = await _responseService.HasRespondedAsync(projectId, email);
            return Ok(exists);
        }


        [HttpDelete("{projectId}/responses/{responseId}")]
        public async Task<IActionResult> DeleteResponse(int projectId, int responseId)
        {
            // Optional: prüfen, ob response zum Projekt gehört
            var resp = await _responseService.GetByIdAsync(responseId);
            if (resp == null || resp.ProjectId != projectId)
                return NotFound();

            await _responseService.DeleteAsync(responseId);
            return NoContent();
        }


    }
}
