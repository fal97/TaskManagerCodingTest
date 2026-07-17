namespace Backend.Domain.Entities;

/// <summary>
/// Base class for all domain entities.
/// Provides common properties and methods for entity tracking.
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// Unique identifier for the entity.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// DateTime when the entity was created.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// DateTime when the entity was last modified.
    /// </summary>
    public DateTime LastModifiedDate { get; set; }

    /// <summary>
    /// Indicates whether the entity has been deleted (soft delete).
    /// </summary>
    public bool IsDeleted { get; set; }
}
