using Microsoft.AspNetCore.Mvc;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Services;

namespace TaskTrack.API.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _service;

    public DepartmentsController(IDepartmentService service)
    {
        _service = service;
    }

    // GET /api/departments
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllActiveAsync();
        return Ok(result);
    }

    // GET /api/departments/search?name=
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest(new { error = "Query parameter 'name' is required." });

        var result = await _service.SearchByNameAsync(name);
        return Ok(result);
    }

    // GET /api/departments/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound(new { error = $"Department {id} not found." });
        return Ok(result);
    }

    // POST /api/departments
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DepartmentCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.DepartmentID }, result);
    }

    // PUT /api/departments/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] DepartmentUpdateDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        if (result == null) return NotFound(new { error = $"Department {id} not found." });
        return Ok(result);
    }

    // DELETE /api/departments/{id}
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

    // GET /api/departments/summary
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var result = await _service.GetSummaryAsync();
        return Ok(result);
    }
}
