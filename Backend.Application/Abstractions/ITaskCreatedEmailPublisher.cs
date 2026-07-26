using Backend.Application.Features.Tasks.DTOs;

namespace Backend.Application.Abstractions;

/// <summary>
/// Publishes task-created email notifications for background processing.
/// </summary>
public interface ITaskCreatedEmailPublisher
{
    Task PublishAsync(
        UserTaskResponse userTask,
        CancellationToken cancellationToken);
}
