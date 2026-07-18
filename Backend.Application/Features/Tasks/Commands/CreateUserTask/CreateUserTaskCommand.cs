using Backend.Application.Features.Tasks.DTOs;
using MediatR;

namespace Backend.Application.Features.Tasks.Commands.CreateUserTask;

/// <summary>
/// Command to create a new user task.
/// </summary>
/// <param name="Request">The create task request containing task details.</param>
public sealed record CreateUserTaskCommand(CreateUserTaskRequest Request)
    : IRequest<UserTaskResponse>;
