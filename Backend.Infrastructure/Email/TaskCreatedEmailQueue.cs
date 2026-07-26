using System.Threading.Channels;
using Backend.Application.Abstractions;
using Backend.Application.Features.Tasks.DTOs;

namespace Backend.Infrastructure.Email;

internal sealed class TaskCreatedEmailQueue : ITaskCreatedEmailQueue
{
    private readonly Channel<UserTaskResponse> _channel =
        Channel.CreateBounded<UserTaskResponse>(new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        });

    public ValueTask QueueAsync(
        UserTaskResponse userTask,
        CancellationToken cancellationToken)
        => _channel.Writer.WriteAsync(userTask, cancellationToken);

    public IAsyncEnumerable<UserTaskResponse> ReadAllAsync(
        CancellationToken cancellationToken)
        => _channel.Reader.ReadAllAsync(cancellationToken);
}
