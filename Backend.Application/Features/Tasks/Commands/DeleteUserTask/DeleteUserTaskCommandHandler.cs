using Backend.Application.Abstractions;
using MediatR;

namespace Backend.Application.Features.Tasks.Commands.DeleteUserTask;

/// <summary>
/// Handler for deleting (soft delete) a user task.
/// </summary>
public class DeleteUserTaskCommandHandler : IRequestHandler<DeleteUserTaskCommand, Unit>
{
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the DeleteUserTaskCommandHandler.
    /// </summary>
    /// <param name="dbContext">The application database context.</param>
    public DeleteUserTaskCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Handles the delete user task command asynchronously.
    /// Performs a soft delete by marking the task as deleted.
    /// </summary>
    /// <param name="request">The delete user task command.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A completed task.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the task is not found.</exception>
    public async Task<Unit> Handle(
        DeleteUserTaskCommand request,
        CancellationToken cancellationToken)
    {
        // Get existing task
        var userTask = await _dbContext.UserTasks.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new KeyNotFoundException($"Task with ID {request.Id} not found.");

        // Soft delete the task
        userTask.IsDeleted = true;

        // Save changes
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
