using System.Security.Cryptography;
using System.Text;
using Backend.Application.Abstractions;
using Microsoft.Extensions.Options;

namespace Backend.Infrastructure.Authentication;

public sealed class SimpleUserCredentialValidator(
    IOptions<SimpleAuthenticationOptions> options) : IUserCredentialValidator
{
    public Task<bool> ValidateAsync(
        string username,
        string password,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var settings = options.Value;
        if (string.IsNullOrWhiteSpace(settings.Username) ||
            string.IsNullOrWhiteSpace(settings.PasswordHash) ||
            string.IsNullOrWhiteSpace(settings.PasswordSalt))
        {
            return Task.FromResult(false);
        }

        var usernameMatches = CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(username),
            Encoding.UTF8.GetBytes(settings.Username));

        var salt = Convert.FromBase64String(settings.PasswordSalt);
        var expectedHash = Convert.FromBase64String(settings.PasswordHash);

        using var deriveBytes = new Rfc2898DeriveBytes(
            password,
            salt,
            settings.Iterations,
            HashAlgorithmName.SHA256);
        var actualHash = deriveBytes.GetBytes(expectedHash.Length);

        return Task.FromResult(
            usernameMatches && CryptographicOperations.FixedTimeEquals(actualHash, expectedHash));
    }
}
