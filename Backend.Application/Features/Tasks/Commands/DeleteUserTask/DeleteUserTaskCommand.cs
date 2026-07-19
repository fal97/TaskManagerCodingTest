using MediatR;

namespace Backend.Application.Features.Tasks.Commands.DeleteUserTask;

/// <summary>
/// Command to delete (soft delete) a user task.
/// </summary>
/// <param name="Id">The ID of the task to delete.</param>
public sealed record DeleteUserTaskCommand(int Id) : IRequest<Unit>;
