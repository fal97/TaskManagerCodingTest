using Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Persistence;

/// <summary>
/// Abstraction for the application database context.
/// Defines the contract for database operations and entity access.
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// Gets the DbSet for UserTask entities.
    /// </summary>
    DbSet<UserTask> UserTasks { get; }

    /// <summary>
    /// Saves all changes made in this context to the database asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
