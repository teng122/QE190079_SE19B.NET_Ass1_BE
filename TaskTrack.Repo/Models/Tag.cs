using System.Collections.Generic;

namespace TaskTrack.Repo.Models;

public class Tag
{
    public int TagID { get; set; }
    public string TagName { get; set; } = null!;
    public string? Color { get; set; }

    public virtual ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
}
