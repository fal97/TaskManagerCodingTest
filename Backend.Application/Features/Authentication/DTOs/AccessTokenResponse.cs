namespace Backend.Application.Features.Authentication.DTOs;

public sealed record AccessTokenResponse(
    string Username,
    string AccessToken,
    DateTime ExpiresAtUtc);
