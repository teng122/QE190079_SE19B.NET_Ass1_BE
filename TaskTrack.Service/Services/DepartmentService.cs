using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories;
using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _repo;
    private readonly IProjectRepository _projectRepo;

    public DepartmentService(IDepartmentRepository repo, IProjectRepository projectRepo)
    {
        _repo = repo;
        _projectRepo = projectRepo;
    }

    public async System.Threading.Tasks.Task<IEnumerable<DepartmentDto>> GetAllActiveAsync()
    {
        var departments = await _repo.GetAllActiveAsync();
        return departments.Select(MapToDto);
    }

    public async System.Threading.Tasks.Task<DepartmentDto?> GetByIdAsync(int id)
    {
        var dept = await _repo.GetByIdWithProjectsAsync(id);
        if (dept == null) return null;
        return MapToDtoWithProjects(dept);
    }

    public async System.Threading.Tasks.Task<IEnumerable<DepartmentDto>> SearchByNameAsync(string name)
    {
        var departments = await _repo.SearchByNameAsync(name);
        return departments.Select(MapToDto);
    }

    public async System.Threading.Tasks.Task<DepartmentDto> CreateAsync(DepartmentCreateDto dto)
    {
        var entity = new Department
        {
            DepartmentName = dto.DepartmentName,
            DepartmentDescription = dto.DepartmentDescription,
            IsActive = dto.IsActive
        };
        var created = await _repo.CreateAsync(entity);
        return MapToDto(created);
    }

    public async System.Threading.Tasks.Task<DepartmentDto?> UpdateAsync(int id, DepartmentUpdateDto dto)
    {
        var entity = new Department
        {
            DepartmentName = dto.DepartmentName,
            DepartmentDescription = dto.DepartmentDescription,
            IsActive = dto.IsActive
        };
        var updated = await _repo.UpdateAsync(id, entity);
        return updated == null ? null : MapToDto(updated);
    }

    public async System.Threading.Tasks.Task<(bool success, string? error)> DeleteAsync(int id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return (false, "Department not found.");

        bool hasProjects = await _repo.HasProjectsAsync(id);
        if (hasProjects) return (false, "Cannot delete department because it has linked projects.");

        await _repo.DeleteAsync(id);
        return (true, null);
    }

    public async System.Threading.Tasks.Task<SummaryDto> GetSummaryAsync()
    {
        var departments = await _repo.GetAllActiveAsync();
        var projects = await _projectRepo.GetAllActiveAsync();
        return new SummaryDto
        {
            TotalDepartments = departments.Count(),
            TotalProjects = projects.Count(),
            TotalTasks = projects.Sum(p => p.Tasks.Count(t => t.IsActive))
        };
    }

    private static DepartmentDto MapToDto(Department d) => new()
    {
        DepartmentID = d.DepartmentID,
        DepartmentName = d.DepartmentName,
        DepartmentDescription = d.DepartmentDescription,
        IsActive = d.IsActive
    };

    private static DepartmentDto MapToDtoWithProjects(Department d) => new()
    {
        DepartmentID = d.DepartmentID,
        DepartmentName = d.DepartmentName,
        DepartmentDescription = d.DepartmentDescription,
        IsActive = d.IsActive,
        Projects = d.Projects.Select(p => new ProjectSummaryDto
        {
            ProjectID = p.ProjectID,
            ProjectName = p.ProjectName,
            Status = p.Status,
            StatusLabel = GetProjectStatusLabel(p.Status),
            IsActive = p.IsActive
        }).ToList()
    };

    public static string GetProjectStatusLabel(short status) => status switch
    {
        0 => "Not Started",
        1 => "In Progress",
        2 => "Completed",
        3 => "On Hold",
        _ => "Unknown"
    };
}
