using FluentValidation;

namespace Backend.Application.Features.Tasks.Commands.UpdateUserTask;

/// <summary>
/// Validator for the UpdateUserTaskCommand.
/// Validates fields and constraints for updating a task.
/// </summary>
public class UpdateUserTaskValidator : AbstractValidator<UpdateUserTaskCommand>
{
    /// <summary>
    /// Initializes a new instance of the UpdateUserTaskValidator.
    /// </summary>
    public UpdateUserTaskValidator()
    {
        RuleFor(x => x.Request.Id)
            .GreaterThan(0)
            .WithMessage("Task ID must be greater than 0.");

        RuleFor(x => x.Request.Title)
            .MaximumLength(200)
            .WithMessage("Title must not exceed 200 characters.")
            .When(x => !string.IsNullOrEmpty(x.Request.Title));

        RuleFor(x => x.Request.Description)
            .MaximumLength(2000)
            .WithMessage("Description must not exceed 2000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Request.Description));

        RuleFor(x => x.Request.Status)
            .InclusiveBetween(0, 4)
            .WithMessage("Status must be between 0 and 4.")
            .When(x => x.Request.Status.HasValue);

        RuleFor(x => x.Request.Priority)
            .InclusiveBetween(0, 3)
            .WithMessage("Priority must be between 0 (Low) and 3 (Critical).")
            .When(x => x.Request.Priority.HasValue);

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
