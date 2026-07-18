namespace Backend.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when a user task cannot be found.
/// </summary>
public sealed class UserTaskNotFoundException(int taskId)
    : NotFoundException($"Task with ID {taskId} not found.");
