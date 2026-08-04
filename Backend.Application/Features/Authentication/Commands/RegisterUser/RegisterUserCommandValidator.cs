using FluentValidation;

namespace Backend.Application.Features.Authentication.Commands.RegisterUser;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(command => command.Request.Username)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(command => command.Request.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(200);

        RuleFor(command => command.Request.ConfirmPassword)
            .Equal(command => command.Request.Password)
            .WithMessage("Passwords do not match.");
    }
}
