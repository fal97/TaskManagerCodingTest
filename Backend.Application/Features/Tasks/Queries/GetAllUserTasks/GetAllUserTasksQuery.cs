using Backend.Application.Features.Tasks.DTOs;
using MediatR;

namespace Backend.Application.Features.Tasks.Queries.GetAllUserTasks;

/// <summary>
/// Query to retrieve all user tasks with optional filtering.
/// </summary>
public class GetAllUserTasksQuery : IRequest<List<UserTaskResponse>>
{
    /// <summary>
    /// Initializes a new instance of the GetAllUserTasksQuery.
    /// </summary>
    /// <param name="status">Optional status filter.</param>
    /// <param name="priority">Optional priority filter.</param>
    public GetAllUserTasksQuery(int? status = null, int? priority = null)
    {
        Status = status;
        Priority = priority;
    }

    /// <summary>
    /// Optional status filter. If provided, only tasks with this status are returned.
    /// </summary>
    public int? Status { get; }

    /// <summary>
    /// Optional priority filter. If provided, only tasks with this priority are returned.
    /// </summary>
    public int? Priority { get; }
}
