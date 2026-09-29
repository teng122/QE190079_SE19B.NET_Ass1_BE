using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories;

public interface ITagRepository
{
    System.Threading.Tasks.Task<IEnumerable<Tag>> GetAllAsync();
    System.Threading.Tasks.Task<Tag?> GetByIdAsync(int id);
    System.Threading.Tasks.Task<Tag> CreateAsync(Tag tag);
    System.Threading.Tasks.Task<Tag?> UpdateAsync(int id, Tag tag);
    System.Threading.Tasks.Task<bool> DeleteAsync(int id);
    System.Threading.Tasks.Task<bool> IsUsedByTaskAsync(int id);
}
