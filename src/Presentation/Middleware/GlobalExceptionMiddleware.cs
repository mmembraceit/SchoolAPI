using System.Net;
using System.Text.Json;
using StudentApi.Domain.ErrorCodes;

namespace StudentApi.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, errorCode, message) = exception switch
        {
            ArgumentNullException       => (HttpStatusCode.BadRequest,            StudentApiErrorCodes.General.InvalidRequest, exception.Message),
            ArgumentException           => (HttpStatusCode.BadRequest,            StudentApiErrorCodes.General.InvalidRequest, exception.Message),
            KeyNotFoundException        => (HttpStatusCode.NotFound,              StudentApiErrorCodes.General.NotFound,       exception.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized,          StudentApiErrorCodes.General.InvalidRequest, "Unauthorized."),
            _                           => (HttpStatusCode.InternalServerError,   StudentApiErrorCodes.General.Unknown,        "An unexpected error occurred.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            statusCode = (int)statusCode,
            errorCode,
            message,
            path = context.Request.Path.Value
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
