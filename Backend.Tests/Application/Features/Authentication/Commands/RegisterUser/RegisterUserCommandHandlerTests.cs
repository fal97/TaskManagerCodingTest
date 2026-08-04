using Backend.Application.Abstractions;
using Backend.Application.Common.Exceptions;
using Backend.Application.Features.Authentication.Commands.RegisterUser;
using Backend.Application.Features.Authentication.DTOs;
using FluentAssertions;
using Moq;

namespace Backend.Tests.Application.Features.Authentication.Commands.RegisterUser;

public class RegisterUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidRegistration_ReturnsAuthenticatedUser()
    {
        var identityService = new Mock<IIdentityService>();
        identityService
            .Setup(service => service.RegisterAsync(
                "new-user",
                "Strong123!",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IdentityRegistrationResult(
                true,
                new Dictionary<string, string[]>()));
        var handler = new RegisterUserCommandHandler(identityService.Object);
        var command = new RegisterUserCommand(new RegisterRequest
        {
            Username = "new-user",
            Password = "Strong123!",
            ConfirmPassword = "Strong123!"
        });

        var response = await handler.Handle(command, CancellationToken.None);

        response.Username.Should().Be("new-user");
    }

    [Fact]
    public async Task Handle_IdentityFailure_ThrowsRequestValidationException()
    {
        var errors = new Dictionary<string, string[]>
        {
            ["DuplicateUserName"] = ["Username is already taken."]
        };
        var identityService = new Mock<IIdentityService>();
        identityService
            .Setup(service => service.RegisterAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IdentityRegistrationResult(false, errors));
        var handler = new RegisterUserCommandHandler(identityService.Object);
        var command = new RegisterUserCommand(new RegisterRequest
        {
            Username = "existing-user",
            Password = "Strong123!",
            ConfirmPassword = "Strong123!"
        });

        var act = async () => await handler.Handle(command, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<RequestValidationException>();
        exception.Which.Errors.Should().BeEquivalentTo(errors);
    }
}
