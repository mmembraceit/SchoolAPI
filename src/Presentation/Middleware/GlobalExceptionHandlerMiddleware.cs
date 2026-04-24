using StudentApi.Api.Models;
using StudentApi.Domain.ErrorCodes;
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
                "Domain exception occurred. ExceptionType={ExceptionType} ErrorCode={ErrorCode} HttpStatus={HttpStatus} Method={Method} Path={Path} TenantId={TenantId} TraceId={TraceId}",
                ex.GetType().Name,
                ex.ErrorCode,
                (int)ex.StatusCode,
                context.Request.Method,
                context.Request.Path.Value,
                GetTenantId(context),
                context.TraceIdentifier);

            context.Response.StatusCode = (int)ex.StatusCode;

            await context.Response.WriteAsJsonAsync(
                ApiResponse<object>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unhandled exception occurred. ExceptionType={ExceptionType} ErrorCode={ErrorCode} HttpStatus={HttpStatus} Method={Method} Path={Path} TenantId={TenantId} TraceId={TraceId}",
                ex.GetType().Name,
                StudentApiErrorCodes.General.Unknown,
                StatusCodes.Status500InternalServerError,
                context.Request.Method,
                context.Request.Path.Value,
                GetTenantId(context),
                context.TraceIdentifier);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(
                ApiResponse<object>.Fail("An unexpected error occurred."));
        }
    }

    private static string GetTenantId(HttpContext context)
    {
        return context.User.FindFirst("tenantId")?.Value ?? "n/a";
    }
}