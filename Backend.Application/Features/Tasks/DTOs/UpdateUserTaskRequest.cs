namespace Backend.Application.Features.Tasks.DTOs;

/// <summary>
/// DTO for updating an existing user task.
/// </summary>
public class UpdateUserTaskRequest
{
    /// <summary>
    /// The ID of the task to update. Required.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The title of the task. Optional for updates.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// The description of the task. Optional.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The status of the task. Optional for updates.
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// The priority of the task. Optional for updates.
    /// </summary>
    public int? Priority { get; set; }

    /// <summary>
    /// The due date for the task. Optional.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Additional notes for the task. Optional.
    /// </summary>
    public string? Notes { get; set; }
}
