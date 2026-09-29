using System.ComponentModel.DataAnnotations;

namespace TaskTrack.Service.DTOs;

// ===== Department DTOs =====
public class DepartmentDto
{
    public int DepartmentID { get; set; }
    public string DepartmentName { get; set; } = null!;
    public string DepartmentDescription { get; set; } = null!;
    public bool IsActive { get; set; }
    public List<ProjectSummaryDto>? Projects { get; set; }
}

public class DepartmentCreateDto
{
    [Required, StringLength(100)]
    public string DepartmentName { get; set; } = null!;
    [Required, StringLength(300)]
    public string DepartmentDescription { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}

public class DepartmentUpdateDto
{
    [Required, StringLength(100)]
    public string DepartmentName { get; set; } = null!;
    [Required, StringLength(300)]
    public string DepartmentDescription { get; set; } = null!;
    public bool IsActive { get; set; }
}

// ===== Project DTOs =====
public class ProjectDto
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = null!;
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public short Status { get; set; }
    public string StatusLabel { get; set; } = null!;
    public int DepartmentID { get; set; }
    public string DepartmentName { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public List<TaskSummaryDto>? Tasks { get; set; }
}

public class ProjectSummaryDto
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = null!;
    public short Status { get; set; }
    public string StatusLabel { get; set; } = null!;
    public bool IsActive { get; set; }
}

public class ProjectCreateDto : IValidatableObject
{
    [Required, StringLength(200)]
    public string ProjectName { get; set; } = null!;
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    [Range(0, 3)]
    public short Status { get; set; } = 0;
    [Range(1, int.MaxValue)]
    public int DepartmentID { get; set; }
    public bool IsActive { get; set; } = true;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate == default)
            yield return new ValidationResult("StartDate is required.", [nameof(StartDate)]);

        if (EndDate.HasValue && EndDate.Value < StartDate)
            yield return new ValidationResult("EndDate must be on or after StartDate.", [nameof(EndDate)]);
    }
}

public class ProjectUpdateDto : IValidatableObject
{
    [Required, StringLength(200)]
    public string ProjectName { get; set; } = null!;
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    [Range(0, 3)]
    public short Status { get; set; }
    [Range(1, int.MaxValue)]
    public int DepartmentID { get; set; }
    public bool IsActive { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate == default)
            yield return new ValidationResult("StartDate is required.", [nameof(StartDate)]);

        if (EndDate.HasValue && EndDate.Value < StartDate)
            yield return new ValidationResult("EndDate must be on or after StartDate.", [nameof(EndDate)]);
    }
}

// ===== Task DTOs =====
public class TaskDto
{
    public int TaskID { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public short Status { get; set; }
    public string StatusLabel { get; set; } = null!;
    public short Priority { get; set; }
    public string PriorityLabel { get; set; } = null!;
    public DateOnly? DueDate { get; set; }
    public int ProjectID { get; set; }
    public string? ProjectName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public List<TagDto>? Tags { get; set; }
}

public class TaskSummaryDto
{
    public int TaskID { get; set; }
    public string Title { get; set; } = null!;
    public short Status { get; set; }
    public string StatusLabel { get; set; } = null!;
    public short Priority { get; set; }
    public string PriorityLabel { get; set; } = null!;
    public DateOnly? DueDate { get; set; }
    public List<TagDto>? Tags { get; set; }
}

public class TaskCreateDto
{
    [Required, StringLength(300)]
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    [Range(0, 3)]
    public short Status { get; set; } = 0;
    [Range(0, 3)]
    public short Priority { get; set; } = 1;
    public DateOnly? DueDate { get; set; }
    [Range(1, int.MaxValue)]
    public int ProjectID { get; set; }
    [MinLength(0)]
    public List<int>? TagIDs { get; set; }
}

public class TaskUpdateDto
{
    [Required, StringLength(300)]
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    [Range(0, 3)]
    public short Status { get; set; }
    [Range(0, 3)]
    public short Priority { get; set; }
    public DateOnly? DueDate { get; set; }
    [Range(1, int.MaxValue)]
    public int ProjectID { get; set; }
    public bool IsActive { get; set; }
    [MinLength(0)]
    public List<int>? TagIDs { get; set; }
}

// ===== Tag DTOs =====
public class TagDto
{
    public int TagID { get; set; }
    public string TagName { get; set; } = null!;
    public string? Color { get; set; }
}

public class TagCreateDto
{
    [Required, StringLength(50)]
    public string TagName { get; set; } = null!;
    [StringLength(7)]
    public string? Color { get; set; }
}

public class TagUpdateDto
{
    [Required, StringLength(50)]
    public string TagName { get; set; } = null!;
    [StringLength(7)]
    public string? Color { get; set; }
}

// ===== Summary DTO =====
public class SummaryDto
{
    public int TotalDepartments { get; set; }
    public int TotalProjects { get; set; }
    public int TotalTasks { get; set; }
}
