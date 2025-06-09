using KiMa_API.Interfaces;
using KiMa_API.Models;
using KiMa_API.Models.Dto;
using Microsoft.EntityFrameworkCore;
using KiMa_API.Data;

namespace KiMa_API.Services
{
    public class ProjectResponseService : IProjectResponseService
    {
        private readonly AppDbContext _ctx;

        public ProjectResponseService(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<ProjectResponse> SubmitResponseAsync(ProjectResponseDto dto)
        {
            // 1. Entity aus DTO anlegen
            var entity = new ProjectResponse
            {
                ProjectId = dto.ProjectId,
                RespondentEmail = dto.RespondentEmail,
                AnswersJson = dto.AnswersJson,
                SubmittedAt = DateTime.Now,                
            };

            // 2. Speichern
            _ctx.Responses.Add(entity);
            await _ctx.SaveChangesAsync();

            return entity;
        }

        public async Task<IEnumerable<ProjectResponse>> GetResponsesForProjectAsync(int projectId)
        {
            return await _ctx.Responses
                             .AsNoTracking()
                             .Where(r => r.ProjectId == projectId)
                             .ToListAsync();
        }
    }
}
