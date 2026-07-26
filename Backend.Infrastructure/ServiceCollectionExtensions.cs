using Backend.Application.Abstractions;
using Backend.Application.Common.Behaviors;
using Backend.Application.Features.Tasks.Commands.CreateUserTask;
using Backend.Infrastructure.Persistence;
using Backend.Infrastructure.Authentication;
using Backend.Infrastructure.Messaging;
using Azure.Messaging.ServiceBus;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Infrastructure;

/// <summary>
/// Extension methods for registering infrastructure services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds infrastructure services to the dependency injection container.
    /// Registers the ApplicationDbContext, MediatR, and FluentValidation.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The application configuration containing connection strings.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when DefaultConnection string is not configured.</exception>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Get the connection string
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");

        // Register DbContext with SQL Server
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, sqlServerOptions =>
                sqlServerOptions.MigrationsAssembly("Backend.Infrastructure")));

        // Register IApplicationDbContext interface for dependency injection
        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        var authenticationSection = configuration.GetSection(
            SimpleAuthenticationOptions.SectionName);
        services.Configure<SimpleAuthenticationOptions>(options =>
        {
            options.Username = authenticationSection[nameof(options.Username)] ?? string.Empty;
            options.PasswordHash = authenticationSection[nameof(options.PasswordHash)] ?? string.Empty;
            options.PasswordSalt = authenticationSection[nameof(options.PasswordSalt)] ?? string.Empty;
            options.Iterations = int.TryParse(
                authenticationSection[nameof(options.Iterations)],
                out var iterations)
                    ? iterations
                    : 100_000;
        });
        services.AddSingleton<IUserCredentialValidator, SimpleUserCredentialValidator>();

        var serviceBusSection = configuration.GetSection(
            ServiceBusOptions.SectionName);
        var serviceBusEnabled = bool.TryParse(
            serviceBusSection[nameof(ServiceBusOptions.Enabled)],
            out var enabled) && enabled;

        if (serviceBusEnabled)
        {
            var serviceBusConnectionString =
                serviceBusSection[nameof(ServiceBusOptions.ConnectionString)];
            var queueName =
                serviceBusSection[nameof(ServiceBusOptions.QueueName)];

            if (string.IsNullOrWhiteSpace(serviceBusConnectionString))
            {
                throw new InvalidOperationException(
                    "ServiceBus:ConnectionString is required when email is enabled.");
            }

            if (string.IsNullOrWhiteSpace(queueName))
            {
                throw new InvalidOperationException(
                    "ServiceBus:QueueName is required when email is enabled.");
            }

            services.Configure<ServiceBusOptions>(options =>
            {
                options.Enabled = true;
                options.ConnectionString = serviceBusConnectionString;
                options.QueueName = queueName;
            });
            services.AddSingleton(new ServiceBusClient(serviceBusConnectionString));
            services.AddSingleton<
                ITaskCreatedEmailPublisher,
                TaskCreatedEmailPublisher>();
        }
        else
        {
            services.AddSingleton<
                ITaskCreatedEmailPublisher,
                NoOpTaskCreatedEmailPublisher>();
        }

        // Register MediatR for CQRS pattern
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateUserTaskCommand).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // Register FluentValidation validators
        services.AddValidatorsFromAssembly(typeof(CreateUserTaskValidator).Assembly);

        return services;
    }
}
