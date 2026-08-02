using Backend.Application.Features.Authentication.DTOs;

namespace Backend.Application.Abstractions;

public interface IAccessTokenGenerator
{
    AccessTokenResponse Generate(string username);
}
