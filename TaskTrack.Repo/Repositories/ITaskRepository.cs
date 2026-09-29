using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories;

public interface ITaskRepository
{
    System.Threading.Tasks.Task<IEnumerable<Models.Task>> GetAllActiveAsync();
    System.Threading.Tasks.Task<Models.Task?> GetByIdWithTagsAsync(int id);
    System.Threading.Tasks.Task<IEnumerable<Models.Task>> GetByProjectAsync(int projectId);
    System.Threading.Tasks.Task<IEnumerable<Models.Task>> SearchAsync(string? title, short? status, short? priority, int? projectId, int? tagId);
    System.Threading.Tasks.Task<Models.Task> CreateAsync(Models.Task task, List<int>? tagIds);
    System.Threading.Tasks.Task<Models.Task?> UpdateAsync(int id, Models.Task task, List<int>? tagIds);
    System.Threading.Tasks.Task<bool> SoftDeleteAsync(int id);
    System.Threading.Tasks.Task<Models.Task?> GetByIdAsync(int id);
}
