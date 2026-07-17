namespace Backend.Domain.Enums;

/// <summary>
/// Represents the priority level of a task.
/// </summary>
public enum TaskPriority
{
    /// <summary>
    /// Task has low priority.
    /// </summary>
    Low = 0,

    /// <summary>
    /// Task has normal priority.
    /// </summary>
    Normal = 1,

    /// <summary>
    /// Task has high priority.
    /// </summary>
    High = 2,

    /// <summary>
    /// Task is critical and needs immediate attention.
    /// </summary>
    Critical = 3
}
