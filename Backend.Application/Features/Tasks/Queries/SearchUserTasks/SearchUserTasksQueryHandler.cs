using Backend.Application.Abstractions;
using Backend.Application.Features.Tasks.DTOs;
using Backend.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backend.Application.Features.Tasks.Queries.SearchUserTasks;

public sealed class SearchUserTasksQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<SearchUserTasksQuery, List<UserTaskResponse>>
{
    public async Task<List<UserTaskResponse>> Handle(
        SearchUserTasksQuery request,
        CancellationToken cancellationToken)
    {
        var query = dbContext.UserTasks.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim();
            query = query.Where(task =>
                task.Title.Contains(searchTerm) ||
                (task.Description != null && task.Description.Contains(searchTerm)) ||
                (task.Notes != null && task.Notes.Contains(searchTerm)));
        }

        if (request.Status.HasValue)
        {
            var status = (Domain.Enums.TaskStatus)request.Status.Value;
            query = query.Where(task => task.Status == status);
        }

        if (request.Priority.HasValue)
        {
            var priority = (Domain.Enums.TaskPriority)request.Priority.Value;
            query = query.Where(task => task.Priority == priority);
        }

        if (request.DueFrom.HasValue)
        {
            query = query.Where(task => task.DueDate >= request.DueFrom.Value);
        }

        if (request.DueTo.HasValue)
        {
            query = query.Where(task => task.DueDate <= request.DueTo.Value);
        }

        query = ApplySorting(query, request.SortBy, request.SortDirection);

        return await query
            .Select(task => new UserTaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = (int)task.Status,
                Priority = (int)task.Priority,
                DueDate = task.DueDate,
                CompletedDate = task.CompletedDate,
                Notes = task.Notes,
                CreatedDate = task.CreatedDate,
                LastModifiedDate = task.LastModifiedDate,
                IsDeleted = task.IsDeleted
            })
            .ToListAsync(cancellationToken);
    }

    private static IQueryable<UserTask> ApplySorting(
        IQueryable<UserTask> query,
        string sortBy,
        string sortDirection)
    {
        var descending = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);

        return sortBy.ToLowerInvariant() switch
        {
            "title" => descending
                ? query.OrderByDescending(task => task.Title)
                : query.OrderBy(task => task.Title),
            "status" => descending
                ? query.OrderByDescending(task => task.Status)
                : query.OrderBy(task => task.Status),
            "priority" => descending
                ? query.OrderByDescending(task => task.Priority)
                : query.OrderBy(task => task.Priority),
            "duedate" => descending
                ? query.OrderByDescending(task => task.DueDate)
                : query.OrderBy(task => task.DueDate),
            "lastmodifieddate" => descending
                ? query.OrderByDescending(task => task.LastModifiedDate)
                : query.OrderBy(task => task.LastModifiedDate),
            _ => descending
                ? query.OrderByDescending(task => task.CreatedDate)
                : query.OrderBy(task => task.CreatedDate)
        };
    }
}
