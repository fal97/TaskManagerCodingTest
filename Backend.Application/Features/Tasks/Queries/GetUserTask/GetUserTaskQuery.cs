using Backend.Application.Features.Tasks.DTOs;
using MediatR;

namespace Backend.Application.Features.Tasks.Queries.GetUserTask;

/// <summary>
/// Query to retrieve a single user task by ID.
/// </summary>
/// <param name="Id">The ID of the task to retrieve.</param>
public sealed record GetUserTaskQuery(int Id) : IRequest<UserTaskResponse>;
