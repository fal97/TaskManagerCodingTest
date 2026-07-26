using Backend.Application.Abstractions;
using Backend.Application.Features.Tasks.DTOs;
using Backend.Domain.Entities;
using MediatR;

namespace Backend.Application.Features.Tasks.Commands.CreateUserTask;

/// <summary>
/// Handler for creating a new user task.
/// </summary>
public class CreateUserTaskCommandHandler : IRequestHandler<CreateUserTaskCommand, UserTaskResponse>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ITaskCreatedEmailQueue _emailQueue;

    /// <summary>
    /// Initializes a new instance of the CreateUserTaskCommandHandler.
    /// </summary>
    /// <param name="dbContext">The application database context.</param>
    public CreateUserTaskCommandHandler(
        IApplicationDbContext dbContext,
        ITaskCreatedEmailQueue emailQueue)
    {
        _dbContext = dbContext;
        _emailQueue = emailQueue;
    }

    /// <summary>
    /// Handles the create user task command asynchronously.
    /// </summary>
    /// <param name="request">The create user task command.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The created user task as a response DTO.</returns>
    public async Task<UserTaskResponse> Handle(
        CreateUserTaskCommand request,
        CancellationToken cancellationToken)
    {
        // Create new UserTask entity
        var userTask = new UserTask
        {
            Title = request.Request.Title,
            Description = request.Request.Description,
            Priority = (Backend.Domain.Enums.TaskPriority)request.Request.Priority,
            DueDate = request.Request.DueDate,
            Notes = request.Request.Notes,
            Status = Domain.Enums.TaskStatus.Pending
        };

        // Add to database
        _dbContext.UserTasks.Add(userTask);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = MapToResponse(userTask);
        await _emailQueue.QueueAsync(response, cancellationToken);

        return response;
    }

    /// <summary>
    /// Maps a UserTask entity to a UserTaskResponse DTO.
    /// </summary>
    private static UserTaskResponse MapToResponse(UserTask userTask)
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
