using Backend.Application.Features.Tasks.DTOs;
using MediatR;

namespace Backend.Application.Features.Tasks.Commands.UpdateUserTask;

/// <summary>
/// Command to update an existing user task.
/// </summary>
/// <param name="Request">The update task request containing task details.</param>
public sealed record UpdateUserTaskCommand(UpdateUserTaskRequest Request)
    : IRequest<UserTaskResponse>;
