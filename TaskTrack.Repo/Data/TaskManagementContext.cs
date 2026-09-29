using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Data;

public class TaskManagementContext : DbContext
{
    public TaskManagementContext(DbContextOptions<TaskManagementContext> options)
        : base(options) { }

    public DbSet<Department> Departments { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Models.Task> Tasks { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<TaskTag> TaskTags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Department
        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Department");
            entity.HasKey(e => e.DepartmentID);
            entity.Property(e => e.DepartmentID).HasColumnName("DepartmentID").UseIdentityAlwaysColumn();
            entity.Property(e => e.DepartmentName).HasColumnName("DepartmentName").HasMaxLength(100).IsRequired();
            entity.Property(e => e.DepartmentDescription).HasColumnName("DepartmentDescription").HasMaxLength(300).IsRequired();
            entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
        });

        // Project
        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("Project");
            entity.HasKey(e => e.ProjectID);
            entity.Property(e => e.ProjectID).HasColumnName("ProjectID").UseIdentityAlwaysColumn();
            entity.Property(e => e.ProjectName).HasColumnName("ProjectName").HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasColumnName("Description");
            entity.Property(e => e.StartDate).HasColumnName("StartDate").IsRequired();
            entity.Property(e => e.EndDate).HasColumnName("EndDate");
            entity.Property(e => e.Status).HasColumnName("Status").HasDefaultValue((short)0);
            entity.Property(e => e.DepartmentID).HasColumnName("DepartmentID").IsRequired();
            entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
            entity.Property(e => e.CreatedDate).HasColumnName("CreatedDate").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Department)
                  .WithMany(d => d.Projects)
                  .HasForeignKey(e => e.DepartmentID)
                  .HasConstraintName("FK_Project_Department");
        });

        // Task
        modelBuilder.Entity<Models.Task>(entity =>
        {
            entity.ToTable("Task");
            entity.HasKey(e => e.TaskID);
            entity.Property(e => e.TaskID).HasColumnName("TaskID").UseIdentityAlwaysColumn();
            entity.Property(e => e.Title).HasColumnName("Title").HasMaxLength(300).IsRequired();
            entity.Property(e => e.Description).HasColumnName("Description");
            entity.Property(e => e.Status).HasColumnName("Status").HasDefaultValue((short)0);
            entity.Property(e => e.Priority).HasColumnName("Priority").HasDefaultValue((short)1);
            entity.Property(e => e.DueDate).HasColumnName("DueDate");
            entity.Property(e => e.ProjectID).HasColumnName("ProjectID").IsRequired();
            entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
            entity.Property(e => e.CreatedDate).HasColumnName("CreatedDate").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.ModifiedDate).HasColumnName("ModifiedDate");

            entity.HasOne(e => e.Project)
                  .WithMany(p => p.Tasks)
                  .HasForeignKey(e => e.ProjectID)
                  .HasConstraintName("FK_Task_Project");
        });

        // Tag
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("Tag");
            entity.HasKey(e => e.TagID);
            entity.Property(e => e.TagID).HasColumnName("TagID").UseIdentityAlwaysColumn();
            entity.Property(e => e.TagName).HasColumnName("TagName").HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.TagName).IsUnique();
            entity.Property(e => e.Color).HasColumnName("Color").HasMaxLength(7);
        });

        // TaskTag
        modelBuilder.Entity<TaskTag>(entity =>
        {
            entity.ToTable("TaskTag");
            entity.HasKey(e => new { e.TaskID, e.TagID });
            entity.Property(e => e.TaskID).HasColumnName("TaskID");
            entity.Property(e => e.TagID).HasColumnName("TagID");

            entity.HasOne(e => e.Task)
                  .WithMany(t => t.TaskTags)
                  .HasForeignKey(e => e.TaskID)
                  .HasConstraintName("FK_TaskTag_Task");

            entity.HasOne(e => e.Tag)
                  .WithMany(t => t.TaskTags)
                  .HasForeignKey(e => e.TagID)
                  .HasConstraintName("FK_TaskTag_Tag");
        });
    }
}
