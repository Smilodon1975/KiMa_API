using KiMa_API.Models;
using KiMa_API.Models.Dto;
using KiMa_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KiMa_API.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpPost]
        public async Task<ActionResult<Project>> Create([FromBody] Project dto)
        {
            var created = await _projectService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetById(int id)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null) return NotFound();
            return Ok(project);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetAll()
        {
            var all = await _projectService.GetAllAsync();
            return Ok(all);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Project dto)
        {
            if (id != dto.Id) return BadRequest();
            var success = await _projectService.UpdateAsync(dto);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _projectService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }


        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] ProjectStatusDto dto)
        {
            var success = await _projectService.UpdateStatusAsync(id, dto);
            if (!success) return NotFound();
            return NoContent();
        }


        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<Project> patch)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null) return NotFound();
            patch.ApplyTo(project);
            await _projectService.UpdateAsync(project);
            return NoContent();
        }

 
    }
}
