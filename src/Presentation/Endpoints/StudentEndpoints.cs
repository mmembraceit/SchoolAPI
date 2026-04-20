using StudentApi.Application.DTOs.Students;
using StudentApi.Application.Services;

namespace StudentApi.Api.Endpoints;

public static class StudentEndpoints
{
    public static WebApplication MapStudentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/students").WithTags("Students");

        group.MapGet("/", async (IStudentService service, CancellationToken ct) =>
        {
            var result = await service.GetAllAsync(ct);
            return Results.Ok(result);
        });

        group.MapGet("/{id:guid}", async (Guid id, IStudentService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        });

        group.MapPost("/", async (StudentCreateRequest request, IStudentService service, CancellationToken ct) =>
        {
            var result = await service.CreateAsync(request, ct);
            return Results.Created($"/api/students/{result.Id}", result);
        });

        group.MapPut("/{id:guid}", async (Guid id, StudentUpdateRequest request, IStudentService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        });

        group.MapDelete("/{id:guid}", async (Guid id, IStudentService service, CancellationToken ct) =>
        {
            var deleted = await service.DeleteAsync(id, ct);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        return app;
    }
}
