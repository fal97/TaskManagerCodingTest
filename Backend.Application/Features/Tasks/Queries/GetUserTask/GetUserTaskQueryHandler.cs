using Backend.Application.Abstractions;
using Backend.Application.Features.Tasks.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backend.Application.Features.Tasks.Queries.GetUserTask;

/// <summary>
/// Handler for retrieving a single user task by ID.
/// </summary>
public class GetUserTaskQueryHandler : IRequestHandler<GetUserTaskQuery, UserTaskResponse>
{
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the GetUserTaskQueryHandler.
    /// </summary>
    /// <param name="dbContext">The application database context.</param>
    public GetUserTaskQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Handles the get user task query asynchronously.
    /// </summary>
    /// <param name="request">The get user task query.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The user task as a response DTO.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the task is not found.</exception>
    public async Task<UserTaskResponse> Handle(
        GetUserTaskQuery request,
        CancellationToken cancellationToken)
    {
        // Get task with AsNoTracking for read-only query
        var userTask = await _dbContext.UserTasks
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Task with ID {request.Id} not found.");

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
