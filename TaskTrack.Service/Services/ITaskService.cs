using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Services;

public interface ITaskService
{
    System.Threading.Tasks.Task<IEnumerable<TaskDto>> GetAllActiveAsync();
    System.Threading.Tasks.Task<TaskDto?> GetByIdAsync(int id);
    System.Threading.Tasks.Task<IEnumerable<TaskDto>> GetByProjectAsync(int projectId);
    System.Threading.Tasks.Task<IEnumerable<TaskDto>> SearchAsync(string? title, short? status, short? priority, int? projectId, int? tagId);
    System.Threading.Tasks.Task<TaskDto> CreateAsync(TaskCreateDto dto);
    System.Threading.Tasks.Task<TaskDto?> UpdateAsync(int id, TaskUpdateDto dto);
    System.Threading.Tasks.Task<IDictionary<string, string[]>> ValidateReferencesAsync(int projectId, List<int>? tagIds);
    System.Threading.Tasks.Task<bool> SoftDeleteAsync(int id);
}
