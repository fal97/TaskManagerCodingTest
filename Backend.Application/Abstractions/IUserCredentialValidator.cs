namespace Backend.Application.Abstractions;

public interface IUserCredentialValidator
{
    Task<bool> ValidateAsync(
        string username,
        string password,
        CancellationToken cancellationToken);
}
