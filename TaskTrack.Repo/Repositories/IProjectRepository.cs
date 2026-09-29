using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories;

public interface IProjectRepository
{
    System.Threading.Tasks.Task<IEnumerable<Project>> GetAllActiveAsync();
    System.Threading.Tasks.Task<Project?> GetByIdWithTasksAsync(int id);
    System.Threading.Tasks.Task<IEnumerable<Project>> GetByDepartmentAsync(int departmentId);
    System.Threading.Tasks.Task<IEnumerable<Project>> SearchAsync(string? name, short? status, int? departmentId);
    System.Threading.Tasks.Task<Project> CreateAsync(Project project);
    System.Threading.Tasks.Task<Project?> UpdateAsync(int id, Project project);
    System.Threading.Tasks.Task<bool> DeleteAsync(int id);
    System.Threading.Tasks.Task<bool> HasTasksAsync(int id);
    System.Threading.Tasks.Task<Project?> GetByIdAsync(int id);
}
