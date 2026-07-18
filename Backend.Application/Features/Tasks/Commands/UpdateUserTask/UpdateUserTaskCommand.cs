using Backend.Application.Features.Tasks.DTOs;
using MediatR;

namespace Backend.Application.Features.Tasks.Commands.UpdateUserTask;

/// <summary>
/// Command to update an existing user task.
/// </summary>
public class UpdateUserTaskCommand : IRequest<UserTaskResponse>
{
    /// <summary>
    /// Initializes a new instance of the UpdateUserTaskCommand.
    /// </summary>
    /// <param name="request">The update task request containing task details.</param>
    public UpdateUserTaskCommand(UpdateUserTaskRequest request)
    {
        Request = request;
    }

    /// <summary>
    /// The update task request containing task details to update.
    /// </summary>
    public UpdateUserTaskRequest Request { get; }
}
