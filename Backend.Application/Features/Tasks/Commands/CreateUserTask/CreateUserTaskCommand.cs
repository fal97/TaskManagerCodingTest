using Backend.Application.Features.Tasks.DTOs;
using MediatR;

namespace Backend.Application.Features.Tasks.Commands.CreateUserTask;

/// <summary>
/// Command to create a new user task.
/// </summary>
public class CreateUserTaskCommand : IRequest<UserTaskResponse>
{
    /// <summary>
    /// Initializes a new instance of the CreateUserTaskCommand.
    /// </summary>
    /// <param name="request">The create task request containing task details.</param>
    public CreateUserTaskCommand(CreateUserTaskRequest request)
    {
        Request = request;
    }

    /// <summary>
    /// The create task request containing task details.
    /// </summary>
    public CreateUserTaskRequest Request { get; }
}
