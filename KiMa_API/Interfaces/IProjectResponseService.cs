﻿using KiMa_API.Models;
using KiMa_API.Models.Dto;

namespace KiMa_API.Interfaces
{
    public interface IProjectResponseService
    {
        Task<ProjectResponse> SubmitResponseAsync(ProjectResponseDto dto);
        Task<IEnumerable<ProjectResponse>> GetResponsesForProjectAsync(int projectId);
        Task<bool> HasRespondedAsync(int projectId, string respondentEmail);
        Task DeleteAsync(int responseId);
        Task<ProjectResponse?> GetByIdAsync(int id);

    }
}
