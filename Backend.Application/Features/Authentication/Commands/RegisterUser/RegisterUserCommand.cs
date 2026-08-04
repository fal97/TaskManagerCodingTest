using Backend.Application.Features.Authentication.DTOs;
using MediatR;

namespace Backend.Application.Features.Authentication.Commands.RegisterUser;

public sealed record RegisterUserCommand(RegisterRequest Request)
    : IRequest<AuthenticatedUserResponse>;
