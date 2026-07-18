using Backend.Application.Features.Tasks.DTOs;
using FluentValidation;

namespace Backend.Application.Features.Tasks.Commands.CreateUserTask;

/// <summary>
/// Validator for the CreateUserTaskRequest.
/// Validates all required fields and constraints for creating a new task.
/// </summary>
public class CreateUserTaskValidator : AbstractValidator<CreateUserTaskRequest>
{
    /// <summary>
    /// Initializes a new instance of the CreateUserTaskValidator.
    /// </summary>
    public CreateUserTaskValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(200)
            .WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Description must not exceed 2000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Priority)
            .InclusiveBetween(0, 3)
            .WithMessage("Priority must be between 0 (Low) and 3 (Critical).");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Due date must be in the future.")
            .When(x => x.DueDate.HasValue);

        RuleFor(x => x.Notes)
            .MaximumLength(2000)
            .WithMessage("Notes must not exceed 2000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}
