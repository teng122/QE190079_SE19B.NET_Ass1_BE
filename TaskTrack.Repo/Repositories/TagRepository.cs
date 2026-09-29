using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Data;
using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories;

public class TagRepository : ITagRepository
{
    private readonly TaskManagementContext _context;

    public TagRepository(TaskManagementContext context)
    {
        _context = context;
    }

    public async System.Threading.Tasks.Task<IEnumerable<Tag>> GetAllAsync()
    {
        return await _context.Tags.OrderBy(t => t.TagName).ToListAsync();
    }

    public async System.Threading.Tasks.Task<Tag?> GetByIdAsync(int id)
    {
        return await _context.Tags.FindAsync(id);
    }

    public async System.Threading.Tasks.Task<Tag> CreateAsync(Tag tag)
    {
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();
        return tag;
    }

    public async System.Threading.Tasks.Task<Tag?> UpdateAsync(int id, Tag tag)
    {
        var existing = await _context.Tags.FindAsync(id);
        if (existing == null) return null;

        existing.TagName = tag.TagName;
        existing.Color = tag.Color;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async System.Threading.Tasks.Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.Tags.FindAsync(id);
        if (existing == null) return false;

        _context.Tags.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }

    public async System.Threading.Tasks.Task<bool> IsUsedByTaskAsync(int id)
    {
        return await _context.TaskTags.AnyAsync(tt => tt.TagID == id);
    }
}
