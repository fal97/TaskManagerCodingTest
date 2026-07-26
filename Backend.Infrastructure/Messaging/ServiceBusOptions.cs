namespace Backend.Infrastructure.Messaging;

public sealed class ServiceBusOptions
{
    public const string SectionName = "ServiceBus";

    public string ConnectionString { get; set; } = string.Empty;

    public string QueueName { get; set; } = "task-created-email";

    public int MaxRetryAttempts { get; set; } = 3;

    public int InitialRetryDelaySeconds { get; set; } = 5;
}
