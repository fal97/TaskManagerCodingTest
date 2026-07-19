namespace Backend.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when an incoming application request fails validation.
/// </summary>
public sealed class RequestValidationException(
    IReadOnlyDictionary<string, string[]> errors)
    : Exception("One or more validation errors occurred.")
{
    public IReadOnlyDictionary<string, string[]> Errors { get; } = errors;
}
