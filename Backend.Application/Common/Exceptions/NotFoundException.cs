namespace Backend.Application.Common.Exceptions;

/// <summary>
/// Base exception for requested application resources that do not exist.
/// </summary>
public abstract class NotFoundException(string message) : Exception(message);
