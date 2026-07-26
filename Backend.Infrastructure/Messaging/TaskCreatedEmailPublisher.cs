using Azure.Messaging.ServiceBus;
using Backend.Application.Abstractions;
using Backend.Application.Features.Tasks.DTOs;
using Microsoft.Extensions.Options;

namespace Backend.Infrastructure.Messaging;

internal sealed class TaskCreatedEmailPublisher(
    ServiceBusClient client,
    IOptions<ServiceBusOptions> options) : ITaskCreatedEmailPublisher, IAsyncDisposable
{
    private readonly ServiceBusSender _sender =
        client.CreateSender(options.Value.QueueName);

    public async Task PublishAsync(
        UserTaskResponse userTask,
        CancellationToken cancellationToken)
    {
        var message = new ServiceBusMessage(
            BinaryData.FromObjectAsJson(userTask))
        {
            ContentType = "application/json",
            MessageId = userTask.Id.ToString(),
            Subject = "TaskCreatedEmail"
        };

        await _sender.SendMessageAsync(message, cancellationToken);
    }

    public ValueTask DisposeAsync() => _sender.DisposeAsync();
}
