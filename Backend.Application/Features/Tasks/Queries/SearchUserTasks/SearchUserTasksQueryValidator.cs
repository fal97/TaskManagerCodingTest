using FluentValidation;

namespace Backend.Application.Features.Tasks.Queries.SearchUserTasks;

public sealed class SearchUserTasksQueryValidator : AbstractValidator<SearchUserTasksQuery>
{
    private static readonly string[] AllowedSortFields =
    [
        "title",
        "status",
        "priority",
        "dueDate",
        "createdDate",
        "lastModifiedDate"
    ];

    public SearchUserTasksQueryValidator()
    {
        RuleFor(query => query.SearchTerm)
            .MaximumLength(200)
            .When(query => !string.IsNullOrWhiteSpace(query.SearchTerm));

        RuleFor(query => query.Status)
            .InclusiveBetween(0, 4)
            .When(query => query.Status.HasValue);

        RuleFor(query => query.Priority)
            .InclusiveBetween(0, 3)
            .When(query => query.Priority.HasValue);

        RuleFor(query => query.DueTo)
            .GreaterThanOrEqualTo(query => query.DueFrom)
            .When(query => query.DueFrom.HasValue && query.DueTo.HasValue)
            .WithMessage("DueTo must be greater than or equal to DueFrom.");

        RuleFor(query => query.SortBy)
            .Must(sortBy => AllowedSortFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"SortBy must be one of: {string.Join(", ", AllowedSortFields)}.");

        RuleFor(query => query.SortDirection)
            .Must(direction =>
                direction.Equals("asc", StringComparison.OrdinalIgnoreCase) ||
                direction.Equals("desc", StringComparison.OrdinalIgnoreCase))
            .WithMessage("SortDirection must be either asc or desc.");
    }
}
