namespace Backend.Domain.Entities;

/// <summary>
/// Represents a user task in the task management system.
/// </summary>
public class UserTask : Entity
{
    /// <summary>
    /// Title or name of the task.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Detailed description of the task.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Current status of the task.
    /// </summary>
    public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.Pending;

    /// <summary>
    /// Priority level of the task.
    /// </summary>
    public Enums.TaskPriority Priority { get; set; } = Enums.TaskPriority.Normal;

    /// <summary>
    /// The date by which the task should be completed.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// DateTime when the task was marked as completed.
    /// Null if task is not yet completed.
    /// </summary>
    public DateTime? CompletedDate { get; set; }

    /// <summary>
    /// Additional notes or comments about the task.
    /// </summary>
    public string? Notes { get; set; }
}
