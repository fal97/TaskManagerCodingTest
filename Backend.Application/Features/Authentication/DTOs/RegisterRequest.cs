namespace Backend.Application.Features.Authentication.DTOs;

public sealed class RegisterRequest
{
    public required string Username { get; set; }

    public required string Password { get; set; }

    public required string ConfirmPassword { get; set; }
}
