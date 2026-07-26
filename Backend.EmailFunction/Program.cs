using Azure.Messaging.ServiceBus;
using Backend.Infrastructure.Email;
using Backend.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        if (!bool.TryParse(
                context.Configuration["Email:Enabled"],
                out var emailEnabled) ||
            !emailEnabled)
        {
            throw new InvalidOperationException(
                "Email:Enabled must be true for the email Function.");
        }

        services.AddEmailServices(context.Configuration);

        var connectionString = context.Configuration["ServiceBusConnection"]
            ?? throw new InvalidOperationException(
                "ServiceBusConnection is required.");
        var queueName = context.Configuration["ServiceBusQueueName"]
            ?? throw new InvalidOperationException(
                "ServiceBusQueueName is required.");
        var maxRetryAttempts = int.TryParse(
            context.Configuration["ServiceBusMaxRetryAttempts"],
            out var configuredMaxRetryAttempts)
                ? configuredMaxRetryAttempts
                : 3;
        var initialRetryDelaySeconds = int.TryParse(
            context.Configuration["ServiceBusInitialRetryDelaySeconds"],
            out var configuredRetryDelaySeconds)
                ? configuredRetryDelaySeconds
                : 5;

        if (maxRetryAttempts < 0)
        {
            throw new InvalidOperationException(
                "ServiceBusMaxRetryAttempts must be zero or greater.");
        }

        if (initialRetryDelaySeconds <= 0)
        {
            throw new InvalidOperationException(
                "ServiceBusInitialRetryDelaySeconds must be greater than zero.");
        }

        services.Configure<ServiceBusOptions>(options =>
        {
            options.Enabled = true;
            options.ConnectionString = connectionString;
            options.QueueName = queueName;
            options.MaxRetryAttempts = maxRetryAttempts;
            options.InitialRetryDelaySeconds = initialRetryDelaySeconds;
        });
        services.AddSingleton(new ServiceBusClient(connectionString));
        services.AddSingleton(provider =>
            provider
                .GetRequiredService<ServiceBusClient>()
                .CreateSender(queueName));
    })
    .Build();

await host.RunAsync();
