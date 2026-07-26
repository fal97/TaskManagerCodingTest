using Backend.Application.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Backend.Infrastructure.Email;

internal sealed class EmailBackgroundService(
    TaskCreatedEmailQueue queue,
    IEmailSender emailSender,
    ILogger<EmailBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var userTask in queue.ReadAllAsync(stoppingToken))
        {
            try
            {
                await emailSender.SendTaskCreatedAsync(userTask, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "Failed to send task-created email for task {TaskId}.",
                    userTask.Id);
            }
        }
    }
}
