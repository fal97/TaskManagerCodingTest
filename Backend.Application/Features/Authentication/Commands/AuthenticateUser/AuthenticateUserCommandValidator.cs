using FluentValidation;

namespace Backend.Application.Features.Authentication.Commands.AuthenticateUser;

public sealed class AuthenticateUserCommandValidator : AbstractValidator<AuthenticateUserCommand>
{
    public AuthenticateUserCommandValidator()
    {
        RuleFor(command => command.Request.Username)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(command => command.Request.Password)
            .NotEmpty()
            .MaximumLength(200);
    }
}
