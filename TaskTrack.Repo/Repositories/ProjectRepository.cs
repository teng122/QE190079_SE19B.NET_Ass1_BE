using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Data;
using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly TaskManagementContext _context;

    public ProjectRepository(TaskManagementContext context)
    {
        _context = context;
    }

    public async System.Threading.Tasks.Task<IEnumerable<Project>> GetAllActiveAsync()
    {
        return await _context.Projects
            .Include(p => p.Department)
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync();
    }

    public async System.Threading.Tasks.Task<Project?> GetByIdAsync(int id)
    {
        return await _context.Projects.FindAsync(id);
    }

    public async System.Threading.Tasks.Task<Project?> GetByIdWithTasksAsync(int id)
    {
        return await _context.Projects
            .Include(p => p.Department)
            .Include(p => p.Tasks.Where(t => t.IsActive))
                .ThenInclude(t => t.TaskTags)
                    .ThenInclude(tt => tt.Tag)
            .FirstOrDefaultAsync(p => p.ProjectID == id);
    }

    public async System.Threading.Tasks.Task<IEnumerable<Project>> GetByDepartmentAsync(int departmentId)
    {
        return await _context.Projects
            .Include(p => p.Department)
            .Where(p => p.IsActive && p.DepartmentID == departmentId)
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync();
    }

    public async System.Threading.Tasks.Task<IEnumerable<Project>> SearchAsync(string? name, short? status, int? departmentId)
    {
        var query = _context.Projects.Include(p => p.Department).Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(p => p.ProjectName.ToLower().Contains(name.ToLower()));

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        if (departmentId.HasValue)
            query = query.Where(p => p.DepartmentID == departmentId.Value);

        return await query.OrderByDescending(p => p.CreatedDate).ToListAsync();
    }

    public async System.Threading.Tasks.Task<Project> CreateAsync(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async System.Threading.Tasks.Task<Project?> UpdateAsync(int id, Project project)
    {
        var existing = await _context.Projects.FindAsync(id);
        if (existing == null) return null;

        existing.ProjectName = project.ProjectName;
        existing.Description = project.Description;
        existing.StartDate = project.StartDate;
        existing.EndDate = project.EndDate;
        existing.Status = project.Status;
        existing.DepartmentID = project.DepartmentID;
        existing.IsActive = project.IsActive;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async System.Threading.Tasks.Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.Projects.FindAsync(id);
        if (existing == null) return false;

        _context.Projects.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }

    public async System.Threading.Tasks.Task<bool> HasTasksAsync(int id)
    {
        return await _context.Tasks.AnyAsync(t => t.ProjectID == id);
    }
}
