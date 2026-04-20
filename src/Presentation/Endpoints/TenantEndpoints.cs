using StudentApi.Application.DTOs.Tenants;
using StudentApi.Application.Services;

namespace StudentApi.Api.Endpoints;

public static class TenantEndpoints
{
    public static WebApplication MapTenantEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/tenants").WithTags("Tenants");

        group.MapGet("/", async (ITenantService service, CancellationToken ct) => 
        {
            var result = await service.GetAllAsync(ct);
            return Results.Ok(result);
        });

        group.MapGet("/{id:guid}", async (Guid id, ITenantService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        });

        group.MapPost("/", async (TenantCreateRequest request, ITenantService service, CancellationToken ct) =>
        {
            var result = await service.CreateAsync(request, ct);
            return Results.Created($"/api/tenants/{result.Id}", result);
        });

        group.MapPut("/{id:guid}", async (Guid id, TenantUpdateRequest request, ITenantService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        });

        group.MapDelete("/{id:guid}", async (Guid id, ITenantService service, CancellationToken ct) =>
        {
            var deleted = await service.DeleteAsync(id, ct);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        return app;
    }
}
