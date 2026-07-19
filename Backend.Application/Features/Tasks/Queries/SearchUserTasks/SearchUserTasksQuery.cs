using Backend.Application.Features.Tasks.DTOs;
using MediatR;

namespace Backend.Application.Features.Tasks.Queries.SearchUserTasks;

/// <summary>
/// Searches, filters, and sorts user tasks.
/// </summary>
public sealed record SearchUserTasksQuery(
    string? SearchTerm = null,
    int? Status = null,
    int? Priority = null,
    DateTime? DueFrom = null,
    DateTime? DueTo = null,
    string SortBy = "createdDate",
    string SortDirection = "desc") : IRequest<List<UserTaskResponse>>;
