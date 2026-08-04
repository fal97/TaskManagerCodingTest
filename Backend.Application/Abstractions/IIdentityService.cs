namespace Backend.Application.Abstractions;

public interface IIdentityService
{
    Task<bool> CheckPasswordAsync(
        string username,
        string password,
        CancellationToken cancellationToken);

    Task<IdentityRegistrationResult> RegisterAsync(
        string username,
        string password,
        CancellationToken cancellationToken);
}

public sealed record IdentityRegistrationResult(
    bool Succeeded,
    IReadOnlyDictionary<string, string[]> Errors);
