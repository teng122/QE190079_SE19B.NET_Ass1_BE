using Microsoft.AspNetCore.Mvc;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Services;

namespace TaskTrack.API.Controllers;

[ApiController]
[Route("api/tags")]
public class TagsController : ControllerBase
{
    private readonly ITagService _service;

    public TagsController(ITagService service)
    {
        _service = service;
    }

    // GET /api/tags
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    // GET /api/tags/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound(new { error = $"Tag {id} not found." });
        return Ok(result);
    }

    // POST /api/tags
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TagCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.TagID }, result);
    }

    // PUT /api/tags/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TagUpdateDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        if (result == null) return NotFound(new { error = $"Tag {id} not found." });
        return Ok(result);
    }

    // DELETE /api/tags/{id}
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
