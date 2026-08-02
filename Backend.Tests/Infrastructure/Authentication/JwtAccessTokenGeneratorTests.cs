using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Infrastructure.Authentication;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace Backend.Tests.Infrastructure.Authentication;

public class JwtAccessTokenGeneratorTests
{
    [Fact]
    public void Generate_ValidUsername_ReturnsSignedTokenWithExpectedClaims()
    {
        var options = Options.Create(new JwtOptions
        {
            Issuer = "TaskManager.Api.Tests",
            Audience = "TaskManager.Frontend.Tests",
            SigningKey = "TaskManager-Test-Signing-Key-At-Least-32-Characters",
            AccessTokenMinutes = 15
        });
        var generator = new JwtAccessTokenGenerator(options);

        var response = generator.Generate("admin");

        var token = new JwtSecurityTokenHandler().ReadJwtToken(response.AccessToken);
        response.Username.Should().Be("admin");
        response.ExpiresAtUtc.Should().BeAfter(DateTime.UtcNow.AddMinutes(14));
        token.Issuer.Should().Be(options.Value.Issuer);
        token.Audiences.Should().ContainSingle(options.Value.Audience);
        token.Claims.Should().Contain(claim =>
            claim.Type == ClaimTypes.Name && claim.Value == "admin");
        token.Claims.Should().Contain(claim => claim.Type == JwtRegisteredClaimNames.Jti);
    }
}
