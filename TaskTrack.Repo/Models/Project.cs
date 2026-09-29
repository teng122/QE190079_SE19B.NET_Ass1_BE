using System;
using System.Collections.Generic;

namespace TaskTrack.Repo.Models;

public class Project
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = null!;
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    /// <summary>0=Not Started, 1=In Progress, 2=Completed, 3=On Hold</summary>
    public short Status { get; set; } = 0;
    public int DepartmentID { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public virtual Department Department { get; set; } = null!;
    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
