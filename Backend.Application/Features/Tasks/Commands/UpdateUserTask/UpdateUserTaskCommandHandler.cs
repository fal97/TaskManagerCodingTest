using Backend.Application.Abstractions;
using Backend.Application.Common.Exceptions;
using Backend.Application.Features.Tasks.DTOs;
using MediatR;

namespace Backend.Application.Features.Tasks.Commands.UpdateUserTask;

/// <summary>
/// Handler for updating an existing user task.
/// </summary>
public class UpdateUserTaskCommandHandler : IRequestHandler<UpdateUserTaskCommand, UserTaskResponse>
{
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the UpdateUserTaskCommandHandler.
    /// </summary>
    /// <param name="dbContext">The application database context.</param>
    public UpdateUserTaskCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Handles the update user task command asynchronously.
    /// </summary>
    /// <param name="request">The update user task command.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The updated user task as a response DTO.</returns>
    /// <exception cref="UserTaskNotFoundException">Thrown when the task is not found.</exception>
    public async Task<UserTaskResponse> Handle(
        UpdateUserTaskCommand request,
        CancellationToken cancellationToken)
    {
        // Get existing task
        var userTask = await _dbContext.UserTasks.FindAsync(new object[] { request.Request.Id }, cancellationToken)
            ?? throw new UserTaskNotFoundException(request.Request.Id);

        // Update properties if provided
        if (!string.IsNullOrEmpty(request.Request.Title))
        {
            userTask.Title = request.Request.Title;
        }

        if (!string.IsNullOrEmpty(request.Request.Description))
        {
            userTask.Description = request.Request.Description;
        }

        if (request.Request.Status.HasValue)
        {
            userTask.Status = (Backend.Domain.Enums.TaskStatus)request.Request.Status.Value;
        }

        if (request.Request.Priority.HasValue)
        {
            userTask.Priority = (Backend.Domain.Enums.TaskPriority)request.Request.Priority.Value;
        }

        if (request.Request.DueDate.HasValue)
        {
            userTask.DueDate = request.Request.DueDate;
        }

        if (!string.IsNullOrEmpty(request.Request.Notes))
        {
            userTask.Notes = request.Request.Notes;
        }

        // Mark as completed if status is Completed
        if (userTask.Status == Domain.Enums.TaskStatus.Completed && !userTask.CompletedDate.HasValue)
        {
            userTask.CompletedDate = DateTime.UtcNow;
        }

        // Save changes
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Map to response DTO
        return MapToResponse(userTask);
    }

    /// <summary>
    /// Maps a UserTask entity to a UserTaskResponse DTO.
    /// </summary>
    private static UserTaskResponse MapToResponse(Backend.Domain.Entities.UserTask userTask)
    {
        return new UserTaskResponse
        {
            Id = userTask.Id,
            Title = userTask.Title,
            Description = userTask.Description,
            Status = (int)userTask.Status,
            Priority = (int)userTask.Priority,
            DueDate = userTask.DueDate,
            CompletedDate = userTask.CompletedDate,
            Notes = userTask.Notes,
            CreatedDate = userTask.CreatedDate,
            LastModifiedDate = userTask.LastModifiedDate,
            IsDeleted = userTask.IsDeleted
        };
    }
}
