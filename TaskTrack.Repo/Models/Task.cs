using System;
using System.Collections.Generic;

namespace TaskTrack.Repo.Models;

public class Task
{
    public int TaskID { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    /// <summary>0=To Do, 1=In Progress, 2=Done, 3=Cancelled</summary>
    public short Status { get; set; } = 0;
    /// <summary>0=Low, 1=Medium, 2=High, 3=Critical</summary>
    public short Priority { get; set; } = 1;
    public DateOnly? DueDate { get; set; }
    public int ProjectID { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; }

    public virtual Project Project { get; set; } = null!;
    public virtual ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
}
