using System.Net;
using System.Text;
using Backend.Application.Abstractions;
using Backend.Application.Features.Tasks.DTOs;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Backend.Infrastructure.Email;

internal sealed class SmtpEmailSender(IOptions<EmailOptions> options) : IEmailSender
{
    private readonly EmailOptions _options = options.Value;

    public async Task SendTaskCreatedAsync(
        UserTaskResponse userTask,
        CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.FromName, _options.FromAddress));
        message.To.Add(MailboxAddress.Parse(_options.RecipientAddress));
        message.Subject = $"New task created: {userTask.Title}";
        message.Body = new BodyBuilder
        {
            HtmlBody = CreateBody(userTask)
        }.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(
            _options.Host,
            _options.Port,
            _options.UseSsl
                ? SecureSocketOptions.StartTls
                : SecureSocketOptions.None,
            cancellationToken);

        if (!string.IsNullOrWhiteSpace(_options.Username))
        {
            await client.AuthenticateAsync(
                _options.Username,
                _options.Password,
                cancellationToken);
        }

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }

    private static string CreateBody(UserTaskResponse userTask)
    {
        var title = WebUtility.HtmlEncode(userTask.Title);
        var description = WebUtility.HtmlEncode(
            userTask.Description ?? "No description");
        var dueDate = WebUtility.HtmlEncode(
            userTask.DueDate?.ToString("u") ?? "No due date");

        return new StringBuilder()
            .Append("<h2>A new task was created</h2>")
            .Append($"<p><strong>Title:</strong> {title}</p>")
            .Append($"<p><strong>Description:</strong> {description}</p>")
            .Append($"<p><strong>Priority:</strong> {userTask.Priority}</p>")
            .Append($"<p><strong>Due:</strong> {dueDate}</p>")
            .Append($"<p><strong>Task ID:</strong> {userTask.Id}</p>")
            .ToString();
    }
}
