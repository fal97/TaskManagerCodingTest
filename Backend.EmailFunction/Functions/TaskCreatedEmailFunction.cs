using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Backend.Application.Abstractions;
using Backend.Application.Features.Tasks.DTOs;
using Backend.Infrastructure.Messaging;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backend.EmailFunction.Functions;

public sealed class TaskCreatedEmailFunction(
    ServiceBusSender sender,
    IEmailSender emailSender,
    IOptions<ServiceBusOptions> options,
    ILogger<TaskCreatedEmailFunction> logger)
{
    private const string OriginalMessageIdProperty = "OriginalMessageId";
    private const string RetryCountProperty = "EmailRetryCount";
    private static readonly JsonSerializerOptions SerializerOptions =
        new(JsonSerializerDefaults.Web);

    [Function(nameof(TaskCreatedEmailFunction))]
    public async Task RunAsync(
        [ServiceBusTrigger(
            "%ServiceBusQueueName%",
            Connection = "ServiceBusConnection")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions,
        CancellationToken cancellationToken)
    {
        UserTaskResponse? userTask;

        try
        {
            userTask = message.Body
                .ToObjectFromJson<UserTaskResponse>(SerializerOptions);
        }
        catch (Exception exception)
            when (exception is JsonException or NotSupportedException)
        {
            logger.LogWarning(
                exception,
                "Dead-lettering malformed email message {MessageId}.",
                message.MessageId);
            await messageActions.DeadLetterMessageAsync(
                message,
                propertiesToModify: null,
                "InvalidPayload",
                exception.Message,
                cancellationToken);
            return;
        }

        if (userTask is null)
        {
            await messageActions.DeadLetterMessageAsync(
                message,
                propertiesToModify: null,
                "InvalidPayload",
                "The message body did not contain a task.",
                cancellationToken);
            return;
        }

        try
        {
            await emailSender.SendTaskCreatedAsync(
                userTask,
                cancellationToken);
            await messageActions.CompleteMessageAsync(
                message,
                cancellationToken);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            await HandleFailureAsync(
                message,
                messageActions,
                exception,
                cancellationToken);
        }
    }

    private async Task HandleFailureAsync(
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var retryCount = GetRetryCount(message);

        if (retryCount >= options.Value.MaxRetryAttempts)
        {
            logger.LogError(
                exception,
                "Dead-lettering email message {MessageId} after {RetryCount} retries.",
                message.MessageId,
                retryCount);
            await messageActions.DeadLetterMessageAsync(
                message,
                propertiesToModify: null,
                "EmailDeliveryFailed",
                Truncate(exception.Message, 4096),
                cancellationToken);
            return;
        }

        var nextRetryCount = retryCount + 1;
        var delaySeconds = Math.Min(
            options.Value.InitialRetryDelaySeconds * Math.Pow(2, retryCount),
            300);
        var retryMessage = CreateRetryMessage(message, nextRetryCount);

        await sender.ScheduleMessageAsync(
            retryMessage,
            DateTimeOffset.UtcNow.AddSeconds(delaySeconds),
            cancellationToken);
        await messageActions.CompleteMessageAsync(
            message,
            cancellationToken);

        logger.LogWarning(
            exception,
            "Scheduled retry {RetryCount}/{MaxRetryAttempts} for email message {MessageId} in {DelaySeconds} seconds.",
            nextRetryCount,
            options.Value.MaxRetryAttempts,
            message.MessageId,
            delaySeconds);
    }

    private static int GetRetryCount(ServiceBusReceivedMessage message)
    {
        if (!message.ApplicationProperties.TryGetValue(
                RetryCountProperty,
                out var value))
        {
            return 0;
        }

        return value switch
        {
            int count => count,
            long count when count <= int.MaxValue => (int)count,
            _ => 0
        };
    }

    private static ServiceBusMessage CreateRetryMessage(
        ServiceBusReceivedMessage original,
        int retryCount)
    {
        var originalMessageId = original.ApplicationProperties.TryGetValue(
            OriginalMessageIdProperty,
            out var storedMessageId)
                ? storedMessageId.ToString() ?? original.MessageId
                : original.MessageId;
        var retryMessage = new ServiceBusMessage(original.Body)
        {
            ContentType = original.ContentType,
            CorrelationId = original.CorrelationId,
            Subject = original.Subject,
            MessageId = $"{originalMessageId}-retry-{retryCount}"
        };

        foreach (var property in original.ApplicationProperties)
        {
            retryMessage.ApplicationProperties[property.Key] = property.Value;
        }

        retryMessage.ApplicationProperties[OriginalMessageIdProperty] =
            originalMessageId;
        retryMessage.ApplicationProperties[RetryCountProperty] = retryCount;
        return retryMessage;
    }

    private static string Truncate(string value, int maxLength)
        => value.Length <= maxLength ? value : value[..maxLength];
}
