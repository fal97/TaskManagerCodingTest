using Backend.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Infrastructure.Email;

public static class EmailServiceCollectionExtensions
{
    public static IServiceCollection AddEmailServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var emailSection = configuration.GetSection(EmailOptions.SectionName);
        var enabled = bool.TryParse(
            emailSection[nameof(EmailOptions.Enabled)],
            out var configuredEnabled) && configuredEnabled;

        services.Configure<EmailOptions>(options =>
        {
            options.Enabled = enabled;
            options.Host = emailSection[nameof(options.Host)] ?? string.Empty;
            options.Port = int.TryParse(
                emailSection[nameof(options.Port)],
                out var port) ? port : 587;
            options.UseSsl = !bool.TryParse(
                emailSection[nameof(options.UseSsl)],
                out var useSsl) || useSsl;
            options.Username =
                emailSection[nameof(options.Username)] ?? string.Empty;
            options.Password =
                emailSection[nameof(options.Password)] ?? string.Empty;
            options.FromAddress =
                emailSection[nameof(options.FromAddress)] ?? string.Empty;
            options.FromName =
                emailSection[nameof(options.FromName)] ?? "Task Manager";
            options.RecipientAddress =
                emailSection[nameof(options.RecipientAddress)] ?? string.Empty;
        });

        if (enabled)
        {
            var requiredSettings = new[]
            {
                nameof(EmailOptions.Host),
                nameof(EmailOptions.FromAddress),
                nameof(EmailOptions.RecipientAddress)
            };
            var missingSetting = requiredSettings.FirstOrDefault(
                setting => string.IsNullOrWhiteSpace(emailSection[setting]));

            if (missingSetting is not null)
            {
                throw new InvalidOperationException(
                    $"Email:{missingSetting} is required when email is enabled.");
            }
        }

        services.AddSingleton<IEmailSender, SmtpEmailSender>();
        return services;
    }
}
