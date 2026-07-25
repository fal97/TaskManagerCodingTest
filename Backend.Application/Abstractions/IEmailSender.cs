using Backend.Application.Features.Tasks.DTOs;

namespace Backend.Application.Abstractions;

/// <summary>
/// Sends application email notifications.
/// </summary>
public interface IEmailSender
{
    Task SendTaskCreatedAsync(
        UserTaskResponse userTask,
        CancellationToken cancellationToken);
}
