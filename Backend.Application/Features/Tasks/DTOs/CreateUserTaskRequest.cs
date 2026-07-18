namespace Backend.Application.Features.Tasks.DTOs;

/// <summary>
/// DTO for creating a new user task.
/// </summary>
public class CreateUserTaskRequest
{
    /// <summary>
    /// The title of the task. Required.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// The description of the task. Optional.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The priority of the task. Defaults to Normal.
    /// </summary>
    public int Priority { get; set; } = 1; // Normal

    /// <summary>
    /// The due date for the task. Optional.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Additional notes for the task. Optional.
    /// </summary>
    public string? Notes { get; set; }
}
