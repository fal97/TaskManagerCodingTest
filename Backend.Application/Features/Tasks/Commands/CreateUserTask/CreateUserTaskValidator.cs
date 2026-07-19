using FluentValidation;

namespace Backend.Application.Features.Tasks.Commands.CreateUserTask;

/// <summary>
/// Validator for the CreateUserTaskCommand.
/// Validates all required fields and constraints for creating a new task.
/// </summary>
public class CreateUserTaskValidator : AbstractValidator<CreateUserTaskCommand>
{
    /// <summary>
    /// Initializes a new instance of the CreateUserTaskValidator.
    /// </summary>
    public CreateUserTaskValidator()
    {
        RuleFor(x => x.Request.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(200)
            .WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Request.Description)
            .MaximumLength(2000)
            .WithMessage("Description must not exceed 2000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Request.Description));

        RuleFor(x => x.Request.Priority)
            .InclusiveBetween(0, 3)
            .WithMessage("Priority must be between 0 (Low) and 3 (Critical).");

        RuleFor(x => x.Request.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Due date must be in the future.")
            .When(x => x.Request.DueDate.HasValue);

        RuleFor(x => x.Request.Notes)
            .MaximumLength(2000)
            .WithMessage("Notes must not exceed 2000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Request.Notes));
    }
}
