using Backend.API.Contracts;
using Backend.Application.Common.Exceptions;

namespace Backend.API.Middleware;

/// <summary>
/// Converts unhandled application exceptions into consistent JSON API responses.
/// </summary>
public sealed class GlobalExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (RequestValidationException exception)
        {
            await WriteValidationErrorAsync(context, exception);
        }
        catch (NotFoundException exception)
        {
            await WriteErrorAsync(
                context,
                StatusCodes.Status404NotFound,
                "Resource not found",
                exception.Message);
        }
        catch (InvalidCredentialsException exception)
        {
            await WriteErrorAsync(
                context,
                StatusCodes.Status401Unauthorized,
                "Authentication failed",
                exception.Message);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "An unhandled exception occurred while processing request {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await WriteErrorAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred",
                "The server was unable to process the request.");
        }
    }

    private static async Task WriteValidationErrorAsync(
        HttpContext context,
        RequestValidationException exception)
    {
        var response = new ValidationErrorResponse(
            StatusCodes.Status400BadRequest,
            exception.Message,
            exception.Errors,
            context.TraceIdentifier);

        context.Response.StatusCode = response.Status;
        await context.Response.WriteAsJsonAsync(response, context.RequestAborted);
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        string title,
        string detail)
    {
        var response = new ErrorResponse(
            statusCode,
            title,
            detail,
            context.TraceIdentifier);

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(response, context.RequestAborted);
    }
}
