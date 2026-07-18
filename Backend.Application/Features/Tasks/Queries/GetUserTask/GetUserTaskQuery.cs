using Backend.Application.Features.Tasks.DTOs;
using MediatR;

namespace Backend.Application.Features.Tasks.Queries.GetUserTask;

/// <summary>
/// Query to retrieve a single user task by ID.
/// </summary>
public class GetUserTaskQuery : IRequest<UserTaskResponse>
{
    /// <summary>
    /// Initializes a new instance of the GetUserTaskQuery.
    /// </summary>
    /// <param name="id">The ID of the task to retrieve.</param>
    public GetUserTaskQuery(int id)
    {
        Id = id;
    }

    /// <summary>
    /// The ID of the task to retrieve.
    /// </summary>
    public int Id { get; }
}
