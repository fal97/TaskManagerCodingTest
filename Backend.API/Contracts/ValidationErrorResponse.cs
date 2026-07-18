namespace Backend.API.Contracts;

/// <summary>
/// Response returned when an API request fails validation.
/// </summary>
public sealed record ValidationErrorResponse(
    int Status,
    string Title,
    IReadOnlyDictionary<string, string[]> Errors,
    string TraceId);
