using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories;

public interface IDepartmentRepository
{
    System.Threading.Tasks.Task<IEnumerable<Department>> GetAllActiveAsync();
    System.Threading.Tasks.Task<Department?> GetByIdWithProjectsAsync(int id);
    System.Threading.Tasks.Task<IEnumerable<Department>> SearchByNameAsync(string name);
    System.Threading.Tasks.Task<Department> CreateAsync(Department department);
    System.Threading.Tasks.Task<Department?> UpdateAsync(int id, Department department);
    System.Threading.Tasks.Task<bool> DeleteAsync(int id);
    System.Threading.Tasks.Task<bool> HasProjectsAsync(int id);
    System.Threading.Tasks.Task<Department?> GetByIdAsync(int id);
}
