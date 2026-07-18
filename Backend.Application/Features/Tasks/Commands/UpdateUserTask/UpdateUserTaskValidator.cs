using Backend.Application.Features.Tasks.DTOs;
using FluentValidation;

namespace Backend.Application.Features.Tasks.Commands.UpdateUserTask;

/// <summary>
/// Validator for the UpdateUserTaskRequest.
/// Validates fields and constraints for updating a task.
/// </summary>
public class UpdateUserTaskValidator : AbstractValidator<UpdateUserTaskRequest>
{
    /// <summary>
    /// Initializes a new instance of the UpdateUserTaskValidator.
    /// </summary>
    public UpdateUserTaskValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Task ID must be greater than 0.");

        RuleFor(x => x.Title)
            .MaximumLength(200)
            .WithMessage("Title must not exceed 200 characters.")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Description must not exceed 2000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Status)
            .InclusiveBetween(0, 4)
            .WithMessage("Status must be between 0 and 4.")
            .When(x => x.Status.HasValue);

        RuleFor(x => x.Priority)
            .InclusiveBetween(0, 3)
            .WithMessage("Priority must be between 0 (Low) and 3 (Critical).")
            .When(x => x.Priority.HasValue);

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
