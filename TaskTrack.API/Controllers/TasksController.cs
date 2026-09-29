using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Services;

namespace TaskTrack.API.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _service;

    public TasksController(ITaskService service)
    {
        _service = service;
    }

    // GET /api/tasks
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllActiveAsync();
        return Ok(result);
    }

    // GET /api/tasks/search?title=&status=&priority=&projectId=&tagId=
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? title,
        [FromQuery, Range(0, 3)] short? status,
        [FromQuery, Range(0, 3)] short? priority,
        [FromQuery, Range(1, int.MaxValue)] int? projectId,
        [FromQuery, Range(1, int.MaxValue)] int? tagId)
    {
        var result = await _service.SearchAsync(title, status, priority, projectId, tagId);
        return Ok(result);
    }

    // GET /api/tasks/project/{projectId}
    [HttpGet("project/{projectId:int}")]
    public async Task<IActionResult> GetByProject(int projectId)
    {
        var result = await _service.GetByProjectAsync(projectId);
        return Ok(result);
    }

    // GET /api/tasks/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound(new { error = $"Task {id} not found." });
        return Ok(result);
    }

    // POST /api/tasks
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TaskCreateDto dto)
    {
        var errors = await _service.ValidateReferencesAsync(dto.ProjectID, dto.TagIDs);
        if (errors.Count > 0)
        {
            foreach (var (field, messages) in errors)
                foreach (var message in messages)
                    ModelState.AddModelError(field, message);

            return ValidationProblem(ModelState);
        }

        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.TaskID }, result);
    }

    // PUT /api/tasks/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TaskUpdateDto dto)
    {
        var errors = await _service.ValidateReferencesAsync(dto.ProjectID, dto.TagIDs);
        if (errors.Count > 0)
        {
            foreach (var (field, messages) in errors)
                foreach (var message in messages)
                    ModelState.AddModelError(field, message);

            return ValidationProblem(ModelState);
        }

        var result = await _service.UpdateAsync(id, dto);
        if (result == null) return NotFound(new { error = $"Task {id} not found." });
        return Ok(result);
    }

    // DELETE /api/tasks/{id} - soft delete
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.SoftDeleteAsync(id);
        if (!success) return NotFound(new { error = $"Task {id} not found." });
        return NoContent();
    }
}
