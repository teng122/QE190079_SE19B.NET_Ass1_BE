using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories;
using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repo;
    private readonly IProjectRepository _projectRepo;
    private readonly ITagRepository _tagRepo;

    public TaskService(ITaskRepository repo, IProjectRepository projectRepo, ITagRepository tagRepo)
    {
        _repo = repo;
        _projectRepo = projectRepo;
        _tagRepo = tagRepo;
    }

    public async System.Threading.Tasks.Task<IEnumerable<TaskDto>> GetAllActiveAsync()
    {
        var tasks = await _repo.GetAllActiveAsync();
        return tasks.Select(MapToDto);
    }

    public async System.Threading.Tasks.Task<TaskDto?> GetByIdAsync(int id)
    {
        var task = await _repo.GetByIdWithTagsAsync(id);
        return task == null ? null : MapToDto(task);
    }

    public async System.Threading.Tasks.Task<IEnumerable<TaskDto>> GetByProjectAsync(int projectId)
    {
        var tasks = await _repo.GetByProjectAsync(projectId);
        return tasks.Select(MapToDto);
    }

    public async System.Threading.Tasks.Task<IEnumerable<TaskDto>> SearchAsync(string? title, short? status, short? priority, int? projectId, int? tagId)
    {
        var tasks = await _repo.SearchAsync(title, status, priority, projectId, tagId);
        return tasks.Select(MapToDto);
    }

    public async System.Threading.Tasks.Task<TaskDto> CreateAsync(TaskCreateDto dto)
    {
        var entity = new Repo.Models.Task
        {
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status,
            Priority = dto.Priority,
            DueDate = dto.DueDate,
            ProjectID = dto.ProjectID
        };
        var created = await _repo.CreateAsync(entity, dto.TagIDs);
        var reloaded = await _repo.GetByIdWithTagsAsync(created.TaskID);
        return MapToDto(reloaded!);
    }

    public async System.Threading.Tasks.Task<TaskDto?> UpdateAsync(int id, TaskUpdateDto dto)
    {
        var entity = new Repo.Models.Task
        {
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status,
            Priority = dto.Priority,
            DueDate = dto.DueDate,
            ProjectID = dto.ProjectID,
            IsActive = dto.IsActive
        };
        var updated = await _repo.UpdateAsync(id, entity, dto.TagIDs);
        if (updated == null) return null;
        var reloaded = await _repo.GetByIdWithTagsAsync(id);
        return reloaded == null ? null : MapToDto(reloaded);
    }

    public async System.Threading.Tasks.Task<bool> SoftDeleteAsync(int id)
    {
        return await _repo.SoftDeleteAsync(id);
    }

    public async System.Threading.Tasks.Task<IDictionary<string, string[]>> ValidateReferencesAsync(
        int projectId, List<int>? tagIds)
    {
        var errors = new Dictionary<string, string[]>();

        if (await _projectRepo.GetByIdAsync(projectId) == null)
            errors[nameof(TaskCreateDto.ProjectID)] = ["ProjectID must reference an existing project."];

        if (tagIds != null)
        {
            var invalidTagIds = new List<int>();
            foreach (var tagId in tagIds.Distinct())
            {
                if (tagId <= 0 || await _tagRepo.GetByIdAsync(tagId) == null)
                    invalidTagIds.Add(tagId);
            }

            if (invalidTagIds.Count > 0)
                errors[nameof(TaskCreateDto.TagIDs)] = ["Every TagID must reference an existing tag."];
        }

        return errors;
    }

    private static TaskDto MapToDto(Repo.Models.Task t) => new()
    {
        TaskID = t.TaskID,
        Title = t.Title,
        Description = t.Description,
        Status = t.Status,
        StatusLabel = GetStatusLabel(t.Status),
        Priority = t.Priority,
        PriorityLabel = GetPriorityLabel(t.Priority),
        DueDate = t.DueDate,
        ProjectID = t.ProjectID,
        ProjectName = t.Project?.ProjectName,
        IsActive = t.IsActive,
        CreatedDate = t.CreatedDate,
        ModifiedDate = t.ModifiedDate,
        Tags = t.TaskTags?.Select(tt => new TagDto
        {
            TagID = tt.Tag.TagID,
            TagName = tt.Tag.TagName,
            Color = tt.Tag.Color
        }).ToList()
    };

    public static string GetStatusLabel(short status) => status switch
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
