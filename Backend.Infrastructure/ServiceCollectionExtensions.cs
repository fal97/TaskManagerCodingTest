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
using Microsoft.AspNetCore.Identity;
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

        services
            .AddIdentityCore<IdentityUser>(options =>
            {
                options.User.RequireUniqueEmail = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();
        services.AddScoped<IIdentityService, IdentityService>();

        var jwtSection = configuration.GetSection(JwtOptions.SectionName);
        services.Configure<JwtOptions>(options =>
        {
            options.Issuer = jwtSection[nameof(options.Issuer)] ?? string.Empty;
            options.Audience = jwtSection[nameof(options.Audience)] ?? string.Empty;
            options.SigningKey = jwtSection[nameof(options.SigningKey)] ?? string.Empty;
            options.AccessTokenMinutes = int.TryParse(
                jwtSection[nameof(options.AccessTokenMinutes)],
                out var accessTokenMinutes)
                    ? accessTokenMinutes
                    : 15;
        });
        services
            .AddOptions<JwtOptions>()
            .Validate(options => !string.IsNullOrWhiteSpace(options.Issuer),
                "Jwt:Issuer is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Audience),
                "Jwt:Audience is required.")
            .Validate(options => options.SigningKey.Length >= 32,
                "Jwt:SigningKey must be at least 32 characters.")
            .Validate(options => options.AccessTokenMinutes > 0,
                "Jwt:AccessTokenMinutes must be greater than zero.")
            .ValidateOnStart();
        services.AddSingleton<IAccessTokenGenerator, JwtAccessTokenGenerator>();

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
