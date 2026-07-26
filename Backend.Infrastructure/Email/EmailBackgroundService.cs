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
    private static readonly JsonSerializerOptions SerializerOptions =
        new(JsonSerializerDefaults.Web);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var processor = client.CreateProcessor(
            options.Value.QueueName,
            new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 1
            });

        processor.ProcessMessageAsync += async args =>
        {
            var userTask = args.Message.Body
                .ToObjectFromJson<UserTaskResponse>(SerializerOptions);

            await emailSender.SendTaskCreatedAsync(
                userTask,
                args.CancellationToken);
            await args.CompleteMessageAsync(
                args.Message,
                args.CancellationToken);
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
}
