using Backend.Application.Abstractions;
using Backend.Application.Common.Exceptions;
using Backend.Application.Features.Authentication.DTOs;
using MediatR;

namespace Backend.Application.Features.Authentication.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler(IIdentityService identityService)
    : IRequestHandler<RegisterUserCommand, AuthenticatedUserResponse>
{
    public async Task<AuthenticatedUserResponse> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var result = await identityService.RegisterAsync(
            request.Request.Username,
            request.Request.Password,
            cancellationToken);

        if (!result.Succeeded)
        {
            throw new RequestValidationException(result.Errors);
        }

        return new AuthenticatedUserResponse(request.Request.Username);
    }
}
