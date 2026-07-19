using Backend.Application.Features.Tasks.Queries.SearchUserTasks;
using FluentAssertions;

namespace Backend.Tests.Application.Features.Tasks.Queries.SearchUserTasks;

public class SearchUserTasksQueryValidatorTests
{
    private readonly SearchUserTasksQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ValidFiltersAndSorting_IsValid()
    {
        var query = new SearchUserTasksQuery(
            SearchTerm: "review",
            Status: 1,
            Priority: 2,
            DueFrom: DateTime.UtcNow,
            DueTo: DateTime.UtcNow.AddDays(7),
            SortBy: "dueDate",
            SortDirection: "asc");

        var result = await _validator.ValidateAsync(query);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_InvalidFiltersAndSorting_ReturnsValidationErrors()
    {
        var query = new SearchUserTasksQuery(
            Status: 5,
            Priority: 4,
            DueFrom: DateTime.UtcNow.AddDays(2),
            DueTo: DateTime.UtcNow,
            SortBy: "unknown",
            SortDirection: "sideways");

        var result = await _validator.ValidateAsync(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Select(error => error.PropertyName).Should().Contain(
            ["Status", "Priority", "DueTo", "SortBy", "SortDirection"]);
    }
}
