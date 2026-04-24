using StudentApi.Api.Models;
using StudentApi.Domain.Exceptions;

namespace StudentApi.Api.Middleware;

internal sealed class GlobalExceptionHandlerMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlerMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ApiException ex)
        {
            logger.LogWarning(
                ex,
                "Domain exception occurred. ErrorCode={ErrorCode} StatusCode={StatusCode} Method={Method} Path={Path} TraceId={TraceId}",
                ex.ErrorCode,
                (int)ex.StatusCode,
                context.Request.Method,
                context.Request.Path.Value,
                context.TraceIdentifier);

            context.Response.StatusCode = (int)ex.StatusCode;

            await context.Response.WriteAsJsonAsync(
                ApiResponse<object>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unhandled exception occurred. Method={Method} Path={Path} TraceId={TraceId}",
                context.Request.Method,
                context.Request.Path.Value,
                context.TraceIdentifier);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(
                ApiResponse<object>.Fail("An unexpected error occurred."));
        }
    }
}