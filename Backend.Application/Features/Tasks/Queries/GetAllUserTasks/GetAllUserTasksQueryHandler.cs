using Backend.Application.Abstractions;
using Backend.Application.Features.Tasks.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backend.Application.Features.Tasks.Queries.GetAllUserTasks;

/// <summary>
/// Handler for retrieving all user tasks with optional filtering.
/// </summary>
public class GetAllUserTasksQueryHandler : IRequestHandler<GetAllUserTasksQuery, List<UserTaskResponse>>
{
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the GetAllUserTasksQueryHandler.
    /// </summary>
    /// <param name="dbContext">The application database context.</param>
    public GetAllUserTasksQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Handles the get all user tasks query asynchronously.
    /// </summary>
    /// <param name="request">The get all user tasks query with optional filters.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of user tasks as response DTOs.</returns>
    public async Task<List<UserTaskResponse>> Handle(
        GetAllUserTasksQuery request,
        CancellationToken cancellationToken)
    {
        // Start with base query using AsNoTracking for read-only optimization
        var query = _dbContext.UserTasks.AsNoTracking();

        // Apply status filter if provided
        if (request.Status.HasValue)
        {
            var status = (Backend.Domain.Enums.TaskStatus)request.Status.Value;
            query = query.Where(x => x.Status == status);
        }

        // Apply priority filter if provided
        if (request.Priority.HasValue)
        {
            var priority = (Backend.Domain.Enums.TaskPriority)request.Priority.Value;
            query = query.Where(x => x.Priority == priority);
        }

        // Execute query and project to DTOs
        var userTasks = await query
            .OrderByDescending(x => x.Priority)
            .ThenBy(x => x.DueDate)
            .ToListAsync(cancellationToken);

        return userTasks.Select(MapToResponse).ToList();
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
