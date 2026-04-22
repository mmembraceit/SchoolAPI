using StudentApi.Api.Models;
using StudentApi.Application.Services;

namespace StudentApi.Api.Endpoints;

public static class AuthEndpoints
{
    // Values for test. 
    private static readonly Guid TestUserId   = Guid.Parse("A0000000-0000-0000-0000-000000000001");
    private static readonly Guid TestTenantId = Guid.Parse("B0000000-0000-0000-0000-000000000002");
    private const string AdminEmail    = "admin@embrace-it.com";
    private const string AdminPassword = "embrace-it1234!";

    public static WebApplication MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/login", (LoginRequest request, ITokenService tokenService) =>
        {
            if (request.Email != AdminEmail || request.Password != AdminPassword)
                return Results.Unauthorized();

            var token = tokenService.GenerateToken(TestTenantId, TestUserId);
            return Results.Ok(ApiResponse<TokenResponse>.Ok(new TokenResponse(token)));
        });

        return app;
    }
}

public sealed record LoginRequest(string Email, string Password);
public sealed record TokenResponse(string Token);
