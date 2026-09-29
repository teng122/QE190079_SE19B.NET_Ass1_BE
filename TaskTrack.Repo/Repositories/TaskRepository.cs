using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Data;
using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TaskManagementContext _context;

    public TaskRepository(TaskManagementContext context)
    {
        _context = context;
    }

    public async System.Threading.Tasks.Task<IEnumerable<Models.Task>> GetAllActiveAsync()
    {
        return await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.TaskTags).ThenInclude(tt => tt.Tag)
            .Where(t => t.IsActive)
            .OrderByDescending(t => t.CreatedDate)
            .ToListAsync();
    }

    public async System.Threading.Tasks.Task<Models.Task?> GetByIdAsync(int id)
    {
        return await _context.Tasks.FindAsync(id);
    }

    public async System.Threading.Tasks.Task<Models.Task?> GetByIdWithTagsAsync(int id)
    {
        return await _context.Tasks
            .Include(t => t.Project).ThenInclude(p => p.Department)
            .Include(t => t.TaskTags).ThenInclude(tt => tt.Tag)
            .FirstOrDefaultAsync(t => t.TaskID == id);
    }

    public async System.Threading.Tasks.Task<IEnumerable<Models.Task>> GetByProjectAsync(int projectId)
    {
        return await _context.Tasks
            .Include(t => t.TaskTags).ThenInclude(tt => tt.Tag)
            .Where(t => t.IsActive && t.ProjectID == projectId)
            .OrderByDescending(t => t.CreatedDate)
            .ToListAsync();
    }

    public async System.Threading.Tasks.Task<IEnumerable<Models.Task>> SearchAsync(string? title, short? status, short? priority, int? projectId, int? tagId)
    {
        var query = _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.TaskTags).ThenInclude(tt => tt.Tag)
            .Where(t => t.IsActive);

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(t => t.Title.ToLower().Contains(title.ToLower()));

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (priority.HasValue)
            query = query.Where(t => t.Priority == priority.Value);

        if (projectId.HasValue)
            query = query.Where(t => t.ProjectID == projectId.Value);

        if (tagId.HasValue)
            query = query.Where(t => t.TaskTags.Any(tt => tt.TagID == tagId.Value));

        return await query.OrderByDescending(t => t.CreatedDate).ToListAsync();
    }

    public async System.Threading.Tasks.Task<Models.Task> CreateAsync(Models.Task task, List<int>? tagIds)
    {
        task.ModifiedDate = DateTime.UtcNow;
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        if (tagIds != null && tagIds.Count > 0)
        {
            foreach (var tagId in tagIds.Distinct())
            {
                _context.TaskTags.Add(new TaskTag { TaskID = task.TaskID, TagID = tagId });
            }
            await _context.SaveChangesAsync();
        }

        return task;
    }

    public async System.Threading.Tasks.Task<Models.Task?> UpdateAsync(int id, Models.Task task, List<int>? tagIds)
    {
        var existing = await _context.Tasks
            .Include(t => t.TaskTags)
            .FirstOrDefaultAsync(t => t.TaskID == id);

        if (existing == null) return null;

        existing.Title = task.Title;
        existing.Description = task.Description;
        existing.Status = task.Status;
        existing.Priority = task.Priority;
        existing.DueDate = task.DueDate;
        existing.ProjectID = task.ProjectID;
        existing.IsActive = task.IsActive;
        existing.ModifiedDate = DateTime.UtcNow;

        // Replace tags
        _context.TaskTags.RemoveRange(existing.TaskTags);

        if (tagIds != null && tagIds.Count > 0)
        {
            foreach (var tagId in tagIds.Distinct())
            {
                _context.TaskTags.Add(new TaskTag { TaskID = id, TagID = tagId });
            }
        }

        await _context.SaveChangesAsync();
        return existing;
    }

    public async System.Threading.Tasks.Task<bool> SoftDeleteAsync(int id)
    {
        var existing = await _context.Tasks.FindAsync(id);
        if (existing == null) return false;

        existing.IsActive = false;
        existing.ModifiedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}
