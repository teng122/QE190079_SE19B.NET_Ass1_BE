using System.Collections.Generic;

namespace TaskTrack.Repo.Models;

public class Department
{
    public int DepartmentID { get; set; }
    public string DepartmentName { get; set; } = null!;
    public string DepartmentDescription { get; set; } = null!;
    public bool IsActive { get; set; } = true;

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
