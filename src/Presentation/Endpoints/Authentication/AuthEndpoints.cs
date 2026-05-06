using StudentApi.Api.Models;
using StudentApi.Application.Repositories;
using StudentApi.Application.Services;

namespace StudentApi.Api.Endpoints;

public static class AuthEndpoints
{
    public static WebApplication MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/login", async (LoginRequest request, IUserRepository userRepository, ITokenService tokenService, CancellationToken ct) =>
        {
            var user = await userRepository.GetByEmailAsync(request.Email, ct);

            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Results.Unauthorized();

            var token = tokenService.GenerateToken(user.TenantId, user.Id, user.Role);
            return Results.Ok(ApiResponse<TokenResponse>.Ok(new TokenResponse(token)));
        });

        return app;
    }
}

public sealed record LoginRequest(string Email, string Password);
public sealed record TokenResponse(string Token);
