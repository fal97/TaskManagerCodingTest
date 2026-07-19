namespace Backend.Infrastructure.Authentication;

public sealed class SimpleAuthenticationOptions
{
    public const string SectionName = "Authentication";

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string PasswordSalt { get; set; } = string.Empty;

    public int Iterations { get; set; } = 100_000;
}
