using Backend.Application.Abstractions;
using Backend.Application.Common.Exceptions;
using Backend.Application.Features.Authentication.Commands.AuthenticateUser;
using Backend.Application.Features.Authentication.DTOs;
using FluentAssertions;
using Moq;

namespace Backend.Tests.Application.Features.Authentication.Commands.AuthenticateUser;

public class AuthenticateUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCredentials_ReturnsAuthenticatedUser()
    {
        var identityService = new Mock<IIdentityService>();
        identityService
            .Setup(service => service.CheckPasswordAsync("admin", "correct", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var handler = new AuthenticateUserCommandHandler(identityService.Object);
        var command = new AuthenticateUserCommand(new LoginRequest
        {
            Username = "admin",
            Password = "correct"
        });

        var response = await handler.Handle(command, CancellationToken.None);

        response.Username.Should().Be("admin");
    }

    [Fact]
    public async Task Handle_InvalidCredentials_ThrowsInvalidCredentialsException()
    {
        var identityService = new Mock<IIdentityService>();
        identityService
            .Setup(service => service.CheckPasswordAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var handler = new AuthenticateUserCommandHandler(identityService.Object);
        var command = new AuthenticateUserCommand(new LoginRequest
        {
            Username = "admin",
            Password = "incorrect"
        });

        var act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }
}
