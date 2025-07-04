﻿using KiMa_API.Interfaces;
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
            var entity = new ProjectResponse
            {
                ProjectId = dto.ProjectId,
                RespondentEmail = dto.RespondentEmail,
                AnswersJson = dto.AnswersJson,
                SubmittedAt = DateTime.Now,
            };
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

        public async Task<bool> HasRespondedAsync(int projectId, string respondentEmail)
        {
            return await _ctx.Responses
                         .AsNoTracking()
                         .AnyAsync(r =>
                             r.ProjectId == projectId &&
                             r.RespondentEmail == respondentEmail);


        }

        public async Task DeleteAsync(int responseId)
        {
            var entity = await _ctx.Responses.FindAsync(responseId);
            if (entity == null) throw new KeyNotFoundException();
            _ctx.Responses.Remove(entity);
            await _ctx.SaveChangesAsync();
        }


        public async Task<ProjectResponse?> GetByIdAsync(int id)
        {
            var response = await _ctx.Responses.FindAsync(id);
            if (response == null)
            {
                return null;
            }
            return response;
        }


    }
}
