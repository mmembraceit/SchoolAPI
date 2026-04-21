using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using StudentApi.Domain.ErrorCodes;
using StudentApi.Domain.Exceptions;

namespace StudentApi.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

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
            if (context.Response.HasStarted)
            {
                throw;
            }

            var (statusCode, errorCode, message) = MapException(ex);

            if ((int)statusCode >= 500)
                _logger.LogError(ex, "Server error {StatusCode} for {Method} {Path}", (int)statusCode, context.Request.Method, context.Request.Path);
            else
                _logger.LogWarning(ex, "Client error {StatusCode} for {Method} {Path}", (int)statusCode, context.Request.Method, context.Request.Path);

            await WriteErrorResponseAsync(context, statusCode, errorCode, message);
        }
    }

    private static (HttpStatusCode statusCode, string errorCode, string message) MapException(Exception exception) =>
        exception switch
        {
            ApiException api                 => (api.StatusCode,                  api.ErrorCode,                               api.Message),
            BadHttpRequestException          => (HttpStatusCode.BadRequest,        StudentApiErrorCodes.General.InvalidRequest, "Invalid request body."),
            ArgumentNullException arg        => (HttpStatusCode.BadRequest,        StudentApiErrorCodes.General.InvalidRequest, arg.Message),
            ArgumentException arg            => (HttpStatusCode.BadRequest,        StudentApiErrorCodes.General.InvalidRequest, arg.Message),
            UnauthorizedAccessException      => (HttpStatusCode.Unauthorized,      StudentApiErrorCodes.General.InvalidRequest, "Unauthorized."),
            DbUpdateException                => (HttpStatusCode.InternalServerError, StudentApiErrorCodes.General.Unknown,      "A database error occurred."),
            _                                => (HttpStatusCode.InternalServerError, StudentApiErrorCodes.General.Unknown,      "An unexpected error occurred.")
        };

    private static async Task WriteErrorResponseAsync(HttpContext context, HttpStatusCode statusCode, string errorCode, string message)
    {
        context.Response.Clear();
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            statusCode = (int)statusCode,
            errorCode,
            message,
            path = context.Request.Path.Value,
            traceId = context.TraceIdentifier
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}
