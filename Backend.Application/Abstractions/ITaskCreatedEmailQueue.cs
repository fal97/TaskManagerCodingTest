using Backend.Application.Features.Tasks.DTOs;

namespace Backend.Application.Abstractions;

/// <summary>
/// Queues task-created email notifications for background processing.
/// </summary>
public interface ITaskCreatedEmailQueue
{
    ValueTask QueueAsync(
        UserTaskResponse userTask,
        CancellationToken cancellationToken);
}
