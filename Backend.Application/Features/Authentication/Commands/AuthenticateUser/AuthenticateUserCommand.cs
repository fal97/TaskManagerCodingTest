using Backend.Application.Features.Authentication.DTOs;
using MediatR;

namespace Backend.Application.Features.Authentication.Commands.AuthenticateUser;

public sealed record AuthenticateUserCommand(LoginRequest Request)
    : IRequest<AuthenticatedUserResponse>;
