using Backend.Application.Abstractions;
using Backend.Application.Features.Tasks.DTOs;

namespace Backend.Infrastructure.Messaging;

internal sealed class NoOpTaskCreatedEmailPublisher : ITaskCreatedEmailPublisher
{
    public Task PublishAsync(
        UserTaskResponse userTask,
        CancellationToken cancellationToken)
        => Task.CompletedTask;
}
