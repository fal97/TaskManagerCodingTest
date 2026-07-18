namespace Backend.Application.Features.Tasks.DTOs;

/// <summary>
/// DTO for returning user task data in API responses.
/// </summary>
public class UserTaskResponse
{
    /// <summary>
    /// The unique identifier of the task.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The title of the task.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// The description of the task.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The current status of the task.
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// The priority level of the task.
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// The due date of the task.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// When the task was completed, if at all.
    /// </summary>
    public DateTime? CompletedDate { get; set; }

    /// <summary>
    /// Additional notes for the task.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// When the task was created.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// When the task was last modified.
    /// </summary>
    public DateTime LastModifiedDate { get; set; }

    /// <summary>
    /// Whether the task is deleted (soft delete).
    /// </summary>
    public bool IsDeleted { get; set; }
}
