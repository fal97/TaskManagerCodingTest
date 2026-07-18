namespace Backend.API.Contracts;

/// <summary>
/// Standard response returned when an API request fails.
/// </summary>
public sealed record ErrorResponse(
    int Status,
    string Title,
    string Detail,
    string TraceId);
