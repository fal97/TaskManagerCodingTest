using Backend.Domain.Entities;
using Backend.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Backend.Infrastructure.Persistence;

/// <summary>
/// Application database context for the Task Management system.
/// Manages the data access layer using Entity Framework Core.
/// </summary>
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    /// <summary>
    /// Initializes a new instance of the ApplicationDbContext class.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the collection of UserTask entities.
    /// </summary>
    public required DbSet<UserTask> UserTasks { get; set; }

    /// <summary>
    /// Configures the model that was discovered by convention from the entity types
    /// exposed in DbSet properties on your context.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    /// <summary>
    /// Saves all changes made in this context to the database asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Updates audit fields (CreatedDate, LastModifiedDate) for entities.
    /// </summary>
    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<Entity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedDate = DateTime.UtcNow;
                entry.Entity.LastModifiedDate = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.LastModifiedDate = DateTime.UtcNow;
            }
        }
    }
}
