using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Application.Abstractions;
using Backend.Application.Features.Authentication.DTOs;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Infrastructure.Authentication;

public sealed class JwtAccessTokenGenerator(IOptions<JwtOptions> options)
    : IAccessTokenGenerator
{
    private readonly JwtOptions _options = options.Value;

    public AccessTokenResponse Generate(string username)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes);
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_options.SigningKey));
        var credentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new AccessTokenResponse(
            username,
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAtUtc);
    }
}
