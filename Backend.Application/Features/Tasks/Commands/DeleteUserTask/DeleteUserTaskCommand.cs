using MediatR;

namespace Backend.Application.Features.Tasks.Commands.DeleteUserTask;

/// <summary>
/// Command to delete (soft delete) a user task.
/// </summary>
public class DeleteUserTaskCommand : IRequest<Unit>
{
    /// <summary>
    /// Initializes a new instance of the DeleteUserTaskCommand.
    /// </summary>
    /// <param name="id">The ID of the task to delete.</param>
    public DeleteUserTaskCommand(int id)
    {
        Id = id;
    }

    /// <summary>
    /// The ID of the task to delete.
    /// </summary>
    public int Id { get; }
}
