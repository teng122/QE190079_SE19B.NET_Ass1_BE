using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories;
using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _repo;
    private readonly IDepartmentRepository _departmentRepo;

    public ProjectService(IProjectRepository repo, IDepartmentRepository departmentRepo)
    {
        _repo = repo;
        _departmentRepo = departmentRepo;
    }

    public async System.Threading.Tasks.Task<IEnumerable<ProjectDto>> GetAllActiveAsync()
    {
        var projects = await _repo.GetAllActiveAsync();
        return projects.Select(MapToDto);
    }

    public async System.Threading.Tasks.Task<ProjectDto?> GetByIdAsync(int id)
    {
        var project = await _repo.GetByIdWithTasksAsync(id);
        return project == null ? null : MapToDtoWithTasks(project);
    }

    public async System.Threading.Tasks.Task<IEnumerable<ProjectDto>> GetByDepartmentAsync(int departmentId)
    {
        var projects = await _repo.GetByDepartmentAsync(departmentId);
        return projects.Select(MapToDto);
    }

    public async System.Threading.Tasks.Task<IEnumerable<ProjectDto>> SearchAsync(string? name, short? status, int? departmentId)
    {
        var projects = await _repo.SearchAsync(name, status, departmentId);
        return projects.Select(MapToDto);
    }

    public async System.Threading.Tasks.Task<ProjectDto> CreateAsync(ProjectCreateDto dto)
    {
        var entity = new Project
        {
            ProjectName = dto.ProjectName,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = dto.Status,
            DepartmentID = dto.DepartmentID,
            IsActive = dto.IsActive
        };
        var created = await _repo.CreateAsync(entity);
        // reload with department
        var reloaded = await _repo.GetByIdWithTasksAsync(created.ProjectID);
        return MapToDto(reloaded!);
    }

    public async System.Threading.Tasks.Task<ProjectDto?> UpdateAsync(int id, ProjectUpdateDto dto)
    {
        var entity = new Project
        {
            ProjectName = dto.ProjectName,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = dto.Status,
            DepartmentID = dto.DepartmentID,
            IsActive = dto.IsActive
        };
        var updated = await _repo.UpdateAsync(id, entity);
        if (updated == null) return null;
        var reloaded = await _repo.GetByIdWithTasksAsync(id);
        return reloaded == null ? null : MapToDto(reloaded);
    }

    public async System.Threading.Tasks.Task<bool> DepartmentExistsAsync(int departmentId)
    {
        return await _departmentRepo.GetByIdAsync(departmentId) != null;
    }

    public async System.Threading.Tasks.Task<(bool success, string? error)> DeleteAsync(int id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return (false, "Project not found.");

        bool hasTasks = await _repo.HasTasksAsync(id);
        if (hasTasks) return (false, "Cannot delete project because it has linked tasks.");

        await _repo.DeleteAsync(id);
        return (true, null);
    }

    private static ProjectDto MapToDto(Project p) => new()
    {
        ProjectID = p.ProjectID,
        ProjectName = p.ProjectName,
        Description = p.Description,
        StartDate = p.StartDate,
        EndDate = p.EndDate,
        Status = p.Status,
        StatusLabel = GetStatusLabel(p.Status),
        DepartmentID = p.DepartmentID,
        DepartmentName = p.Department?.DepartmentName ?? "",
        IsActive = p.IsActive,
        CreatedDate = p.CreatedDate
    };

    private static ProjectDto MapToDtoWithTasks(Project p) => new()
    {
        ProjectID = p.ProjectID,
        ProjectName = p.ProjectName,
        Description = p.Description,
        StartDate = p.StartDate,
        EndDate = p.EndDate,
        Status = p.Status,
        StatusLabel = GetStatusLabel(p.Status),
        DepartmentID = p.DepartmentID,
        DepartmentName = p.Department?.DepartmentName ?? "",
        IsActive = p.IsActive,
        CreatedDate = p.CreatedDate,
        Tasks = p.Tasks.Select(t => new TaskSummaryDto
        {
            TaskID = t.TaskID,
            Title = t.Title,
            Status = t.Status,
            StatusLabel = GetTaskStatusLabel(t.Status),
            Priority = t.Priority,
            PriorityLabel = GetPriorityLabel(t.Priority),
            DueDate = t.DueDate,
            Tags = t.TaskTags.Select(tt => new TagDto
            {
                TagID = tt.Tag.TagID,
                TagName = tt.Tag.TagName,
                Color = tt.Tag.Color
            }).ToList()
        }).ToList()
    };

    public static string GetStatusLabel(short status) => status switch
    {
        0 => "Not Started",
        1 => "In Progress",
        2 => "Completed",
        3 => "On Hold",
        _ => "Unknown"
    };

    public static string GetTaskStatusLabel(short status) => status switch
    {
        0 => "To Do",
        1 => "In Progress",
        2 => "Done",
        3 => "Cancelled",
        _ => "Unknown"
    };

    public static string GetPriorityLabel(short priority) => priority switch
    {
        0 => "Low",
        1 => "Medium",
        2 => "High",
        3 => "Critical",
        _ => "Unknown"
    };
}
