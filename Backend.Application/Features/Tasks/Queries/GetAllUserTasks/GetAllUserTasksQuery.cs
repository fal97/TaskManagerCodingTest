using Backend.Application.Features.Tasks.DTOs;
using MediatR;

namespace Backend.Application.Features.Tasks.Queries.GetAllUserTasks;

/// <summary>
/// Query to retrieve all user tasks with optional filtering.
/// </summary>
/// <param name="Status">Optional status filter.</param>
/// <param name="Priority">Optional priority filter.</param>
public sealed record GetAllUserTasksQuery(
    int? Status = null,
    int? Priority = null) : IRequest<List<UserTaskResponse>>;
