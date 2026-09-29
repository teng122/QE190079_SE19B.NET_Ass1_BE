namespace TaskTrack.Repo.Models;

public class TaskTag
{
    public int TaskID { get; set; }
    public int TagID { get; set; }

    public virtual Task Task { get; set; } = null!;
    public virtual Tag Tag { get; set; } = null!;
}
