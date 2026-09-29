using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Services;

public interface IProjectService
{
    System.Threading.Tasks.Task<IEnumerable<ProjectDto>> GetAllActiveAsync();
    System.Threading.Tasks.Task<ProjectDto?> GetByIdAsync(int id);
    System.Threading.Tasks.Task<IEnumerable<ProjectDto>> GetByDepartmentAsync(int departmentId);
    System.Threading.Tasks.Task<IEnumerable<ProjectDto>> SearchAsync(string? name, short? status, int? departmentId);
    System.Threading.Tasks.Task<ProjectDto> CreateAsync(ProjectCreateDto dto);
    System.Threading.Tasks.Task<ProjectDto?> UpdateAsync(int id, ProjectUpdateDto dto);
    System.Threading.Tasks.Task<bool> DepartmentExistsAsync(int departmentId);
    System.Threading.Tasks.Task<(bool success, string? error)> DeleteAsync(int id);
}
