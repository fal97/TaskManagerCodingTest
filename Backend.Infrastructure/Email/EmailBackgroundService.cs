using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Backend.Application.Abstractions;
using Backend.Application.Features.Tasks.DTOs;
using Backend.Infrastructure.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backend.Infrastructure.Email;

internal sealed class EmailBackgroundService(
    ServiceBusClient client,
    IOptions<ServiceBusOptions> options,
    IEmailSender emailSender,
    ILogger<EmailBackgroundService> logger) : BackgroundService
{
    private const string OriginalMessageIdProperty = "OriginalMessageId";
    private const string RetryCountProperty = "EmailRetryCount";
    private static readonly JsonSerializerOptions SerializerOptions =
        new(JsonSerializerDefaults.Web);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var sender = client.CreateSender(options.Value.QueueName);
        await using var processor = client.CreateProcessor(
            options.Value.QueueName,
            new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 1
            });

        processor.ProcessMessageAsync += async args =>
        {
            UserTaskResponse? userTask;

            try
            {
                userTask = args.Message.Body
                    .ToObjectFromJson<UserTaskResponse>(SerializerOptions);
            }
            catch (Exception exception)
                when (exception is JsonException or NotSupportedException)
            {
                logger.LogWarning(
                    exception,
                    "Dead-lettering malformed email message {MessageId}.",
                    args.Message.MessageId);
                await args.DeadLetterMessageAsync(
                    args.Message,
                    "InvalidPayload",
                    exception.Message,
                    args.CancellationToken);
                return;
            }

            if (userTask is null)
            {
                await args.DeadLetterMessageAsync(
                    args.Message,
                    "InvalidPayload",
                    "The message body did not contain a task.",
                    args.CancellationToken);
                return;
            }

            try
            {
                await emailSender.SendTaskCreatedAsync(
                    userTask,
                    args.CancellationToken);
                await args.CompleteMessageAsync(
                    args.Message,
                    args.CancellationToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                await HandleFailureAsync(args, sender, exception);
            }
        };

        processor.ProcessErrorAsync += args =>
        {
            logger.LogError(
                args.Exception,
                "Azure Service Bus email consumer failed for {EntityPath}.",
                args.EntityPath);
            return Task.CompletedTask;
        };

        await processor.StartProcessingAsync(stoppingToken);

        try
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Normal application shutdown.
        }

        await processor.StopProcessingAsync(CancellationToken.None);
    }

    private async Task HandleFailureAsync(
        ProcessMessageEventArgs args,
        ServiceBusSender sender,
        Exception exception)
    {
        var retryCount = GetRetryCount(args.Message);

        if (retryCount >= options.Value.MaxRetryAttempts)
        {
            logger.LogError(
                exception,
                "Dead-lettering email message {MessageId} after {RetryCount} retries.",
                args.Message.MessageId,
                retryCount);
            await args.DeadLetterMessageAsync(
                args.Message,
                "EmailDeliveryFailed",
                Truncate(exception.Message, 4096),
                args.CancellationToken);
            return;
        }

        var nextRetryCount = retryCount + 1;
        var delaySeconds = Math.Min(
            options.Value.InitialRetryDelaySeconds * Math.Pow(2, retryCount),
            300);
        var delay = TimeSpan.FromSeconds(delaySeconds);
        var retryMessage = CreateRetryMessage(args.Message, nextRetryCount);

        await sender.ScheduleMessageAsync(
            retryMessage,
            DateTimeOffset.UtcNow.Add(delay),
            args.CancellationToken);
        await args.CompleteMessageAsync(
            args.Message,
            args.CancellationToken);

        logger.LogWarning(
            exception,
            "Scheduled retry {RetryCount}/{MaxRetryAttempts} for email message {MessageId} in {DelaySeconds} seconds.",
            nextRetryCount,
            options.Value.MaxRetryAttempts,
            args.Message.MessageId,
            delay.TotalSeconds);
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
