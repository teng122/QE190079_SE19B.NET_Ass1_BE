using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Data;
using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly TaskManagementContext _context;

    public DepartmentRepository(TaskManagementContext context)
    {
        _context = context;
    }

    public async System.Threading.Tasks.Task<IEnumerable<Department>> GetAllActiveAsync()
    {
        return await _context.Departments
            .Where(d => d.IsActive)
            .OrderBy(d => d.DepartmentName)
            .ToListAsync();
    }

    public async System.Threading.Tasks.Task<Department?> GetByIdAsync(int id)
    {
        return await _context.Departments.FindAsync(id);
    }

    public async System.Threading.Tasks.Task<Department?> GetByIdWithProjectsAsync(int id)
    {
        return await _context.Departments
            .Include(d => d.Projects.Where(p => p.IsActive))
            .FirstOrDefaultAsync(d => d.DepartmentID == id);
    }

    public async System.Threading.Tasks.Task<IEnumerable<Department>> SearchByNameAsync(string name)
    {
        return await _context.Departments
            .Where(d => d.IsActive && d.DepartmentName.ToLower().Contains(name.ToLower()))
            .OrderBy(d => d.DepartmentName)
            .ToListAsync();
    }

    public async System.Threading.Tasks.Task<Department> CreateAsync(Department department)
    {
        _context.Departments.Add(department);
        await _context.SaveChangesAsync();
        return department;
    }

    public async System.Threading.Tasks.Task<Department?> UpdateAsync(int id, Department department)
    {
        var existing = await _context.Departments.FindAsync(id);
        if (existing == null) return null;

        existing.DepartmentName = department.DepartmentName;
        existing.DepartmentDescription = department.DepartmentDescription;
        existing.IsActive = department.IsActive;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async System.Threading.Tasks.Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.Departments.FindAsync(id);
        if (existing == null) return false;

        _context.Departments.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }

    public async System.Threading.Tasks.Task<bool> HasProjectsAsync(int id)
    {
        return await _context.Projects.AnyAsync(p => p.DepartmentID == id);
    }
}
