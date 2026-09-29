using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Services;

public interface ITagService
{
    System.Threading.Tasks.Task<IEnumerable<TagDto>> GetAllAsync();
    System.Threading.Tasks.Task<TagDto?> GetByIdAsync(int id);
    System.Threading.Tasks.Task<TagDto> CreateAsync(TagCreateDto dto);
    System.Threading.Tasks.Task<TagDto?> UpdateAsync(int id, TagUpdateDto dto);
    System.Threading.Tasks.Task<(bool success, string? error)> DeleteAsync(int id);
}
