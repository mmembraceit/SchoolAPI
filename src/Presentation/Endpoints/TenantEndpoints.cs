using StudentApi.Api.Models;
using StudentApi.Application.DTOs.Tenants;
using StudentApi.Application.Services;

namespace StudentApi.Api.Endpoints;

public static class TenantEndpoints
{
    public static WebApplication MapTenantEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/tenants").WithTags("Tenants").RequireAuthorization();

        group.MapGet("/", async (ITenantService service, CancellationToken ct) =>
        {
            var result = await service.GetAllAsync(ct);
            return Results.Ok(ApiResponse<IReadOnlyCollection<TenantResponse>>.Ok(result));
        });

        group.MapGet("/{id:guid}", async (Guid id, ITenantService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return Results.Ok(ApiResponse<TenantResponse>.Ok(result));
        });

        group.MapPost("/", async (TenantCreateRequest request, ITenantService service, CancellationToken ct) =>
        {
            var result = await service.CreateAsync(request, ct);
            return Results.Created($"/api/tenants/{result.Id}", ApiResponse<TenantResponse>.Ok(result));
        });

        group.MapPut("/{id:guid}", async (Guid id, TenantUpdateRequest request, ITenantService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return Results.Ok(ApiResponse<TenantResponse>.Ok(result));
        });

        group.MapDelete("/{id:guid}", async (Guid id, ITenantService service, CancellationToken ct) =>
        {
            await service.DeleteAsync(id, ct);
            return Results.NoContent();
        });

        return app;
    }
}
