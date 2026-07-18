using Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for the UserTask entity using Fluent API.
/// Defines table structure, constraints, and indexing.
/// </summary>
public class UserTaskConfiguration : IEntityTypeConfiguration<UserTask>
{
    /// <summary>
    /// Configures the UserTask entity using the Fluent API.
    /// </summary>
    /// <param name="builder">The entity type builder used to configure the entity.</param>
    public void Configure(EntityTypeBuilder<UserTask> builder)
    {
        // Table configuration
        builder.ToTable("UserTasks", "dbo");

        // Primary key
        builder.HasKey(x => x.Id);

        // Property configurations
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("Id");

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("Title");

        builder.Property(x => x.Description)
            .HasMaxLength(2000)
            .HasColumnName("Description");

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasColumnName("Status");

        builder.Property(x => x.Priority)
            .IsRequired()
            .HasConversion<int>()
            .HasColumnName("Priority");

        builder.Property(x => x.DueDate)
            .HasColumnName("DueDate");

        builder.Property(x => x.CompletedDate)
            .HasColumnName("CompletedDate");

        builder.Property(x => x.Notes)
            .HasMaxLength(2000)
            .HasColumnName("Notes");

        builder.Property(x => x.CreatedDate)
            .IsRequired()
            .HasColumnName("CreatedDate");

        builder.Property(x => x.LastModifiedDate)
            .IsRequired()
            .HasColumnName("LastModifiedDate");

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnName("IsDeleted");

        // Indexes
        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_UserTasks_Status");

        builder.HasIndex(x => x.Priority)
            .HasDatabaseName("IX_UserTasks_Priority");

        builder.HasIndex(x => x.DueDate)
            .HasDatabaseName("IX_UserTasks_DueDate");

        builder.HasIndex(x => x.IsDeleted)
            .HasDatabaseName("IX_UserTasks_IsDeleted");

        // Query filter for soft delete
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
