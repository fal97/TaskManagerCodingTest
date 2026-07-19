using Backend.Application.Abstractions;
using Backend.Application.Common.Exceptions;
using Backend.Application.Features.Authentication.DTOs;
using MediatR;

namespace Backend.Application.Features.Authentication.Commands.AuthenticateUser;

public sealed class AuthenticateUserCommandHandler(IUserCredentialValidator credentialValidator)
    : IRequestHandler<AuthenticateUserCommand, AuthenticatedUserResponse>
{
    public async Task<AuthenticatedUserResponse> Handle(
        AuthenticateUserCommand request,
        CancellationToken cancellationToken)
    {
        var isValid = await credentialValidator.ValidateAsync(
            request.Request.Username,
            request.Request.Password,
            cancellationToken);

        if (!isValid)
        {
            throw new InvalidCredentialsException();
        }

        return new AuthenticatedUserResponse(request.Request.Username);
    }
}
