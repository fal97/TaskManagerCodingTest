namespace Backend.Domain.Enums;

/// <summary>
/// Represents the status of a task.
/// </summary>
public enum TaskStatus
{
    /// <summary>
    /// Task is newly created and not yet started.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Task is currently in progress.
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Task has been completed successfully.
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Task has been cancelled and will not be completed.
    /// </summary>
    Cancelled = 3,

    /// <summary>
    /// Task is on hold temporarily.
    /// </summary>
    OnHold = 4
}
