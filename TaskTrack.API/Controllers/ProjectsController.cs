using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Services;

namespace TaskTrack.API.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _service;

    public ProjectsController(IProjectService service)
    {
        _service = service;
    }

    // GET /api/projects
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllActiveAsync();
        return Ok(result);
    }

    // GET /api/projects/search?name=&status=&departmentId=
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? name,
        [FromQuery, Range(0, 3)] short? status,
        [FromQuery, Range(1, int.MaxValue)] int? departmentId)
    {
        var result = await _service.SearchAsync(name, status, departmentId);
        return Ok(result);
    }

    // GET /api/projects/department/{departmentId}
    [HttpGet("department/{departmentId:int}")]
    public async Task<IActionResult> GetByDepartment(int departmentId)
    {
        var result = await _service.GetByDepartmentAsync(departmentId);
        return Ok(result);
    }

    // GET /api/projects/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound(new { error = $"Project {id} not found." });
        return Ok(result);
    }

    // POST /api/projects
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProjectCreateDto dto)
    {
        if (!await _service.DepartmentExistsAsync(dto.DepartmentID))
        {
            ModelState.AddModelError(nameof(dto.DepartmentID), "DepartmentID must reference an existing department.");
            return ValidationProblem(ModelState);
        }

        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.ProjectID }, result);
    }

    // PUT /api/projects/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProjectUpdateDto dto)
    {
        if (!await _service.DepartmentExistsAsync(dto.DepartmentID))
        {
            ModelState.AddModelError(nameof(dto.DepartmentID), "DepartmentID must reference an existing department.");
            return ValidationProblem(ModelState);
        }

        var result = await _service.UpdateAsync(id, dto);
        if (result == null) return NotFound(new { error = $"Project {id} not found." });
        return Ok(result);
    }

    // DELETE /api/projects/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, error) = await _service.DeleteAsync(id);
        if (!success)
        {
            if (error!.Contains("not found")) return NotFound(new { error });
            return BadRequest(new { error });
        }
        return NoContent();
    }
}
