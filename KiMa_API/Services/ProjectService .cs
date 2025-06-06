using KiMa_API.Data;
using KiMa_API.Models;
using KiMa_API.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KiMa_API.Services
{
    public class ProjectService : IProjectService
    {
        private readonly AppDbContext _ctx;

        public ProjectService(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Project> CreateAsync(Project project)
        {
            _ctx.Projects.Add(project);
            await _ctx.SaveChangesAsync();
            return project;
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _ctx.Projects.FindAsync(id);
        }

        public async Task<IEnumerable<Project>> GetAllAsync()
        {
            return await _ctx.Projects
                             .AsNoTracking()
                             .ToListAsync();
        }

        public async Task<bool> UpdateAsync(Project project)
        {
            var exists = await _ctx.Projects.AnyAsync(p => p.Id == project.Id);
            if (!exists) return false;

            _ctx.Projects.Update(project);
            await _ctx.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var project = await _ctx.Projects.FindAsync(id);
            if (project == null) return false;

            _ctx.Projects.Remove(project);
            await _ctx.SaveChangesAsync();
            return true;
        }        
    }
}
