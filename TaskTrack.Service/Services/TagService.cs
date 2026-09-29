using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories;
using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Services;

public class TagService : ITagService
{
    private readonly ITagRepository _repo;

    public TagService(ITagRepository repo)
    {
        _repo = repo;
    }

    public async System.Threading.Tasks.Task<IEnumerable<TagDto>> GetAllAsync()
    {
        var tags = await _repo.GetAllAsync();
        return tags.Select(MapToDto);
    }

    public async System.Threading.Tasks.Task<TagDto?> GetByIdAsync(int id)
    {
        var tag = await _repo.GetByIdAsync(id);
        return tag == null ? null : MapToDto(tag);
    }

    public async System.Threading.Tasks.Task<TagDto> CreateAsync(TagCreateDto dto)
    {
        var entity = new Tag
        {
            TagName = dto.TagName,
            Color = dto.Color
        };
        var created = await _repo.CreateAsync(entity);
        return MapToDto(created);
    }

    public async System.Threading.Tasks.Task<TagDto?> UpdateAsync(int id, TagUpdateDto dto)
    {
        var entity = new Tag
        {
            TagName = dto.TagName,
            Color = dto.Color
        };
        var updated = await _repo.UpdateAsync(id, entity);
        return updated == null ? null : MapToDto(updated);
    }

    public async System.Threading.Tasks.Task<(bool success, string? error)> DeleteAsync(int id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return (false, "Tag not found.");

        bool isUsed = await _repo.IsUsedByTaskAsync(id);
        if (isUsed) return (false, "Cannot delete tag because it is used by one or more tasks.");

        await _repo.DeleteAsync(id);
        return (true, null);
    }

    private static TagDto MapToDto(Tag t) => new()
    {
        TagID = t.TagID,
        TagName = t.TagName,
        Color = t.Color
    };
}
