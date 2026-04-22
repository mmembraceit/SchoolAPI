using StudentApi.Api.Models;
using StudentApi.Application.DTOs.Students;
using StudentApi.Application.Services;

namespace StudentApi.Api.Endpoints;

public static class StudentEndpoints
{
    public static WebApplication MapStudentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/students").WithTags("Students").RequireAuthorization();

        group.MapGet("/", async (IStudentService service, CancellationToken ct) =>
        {
            var result = await service.GetAllAsync(ct);
            return Results.Ok(ApiResponse<IReadOnlyCollection<StudentResponse>>.Ok(result));
        });

        group.MapGet("/{id:guid}", async (Guid id, IStudentService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return Results.Ok(ApiResponse<StudentResponse>.Ok(result));
        });

        group.MapPost("/", async (StudentCreateRequest request, IStudentService service, CancellationToken ct) =>
        {
            var result = await service.CreateAsync(request, ct);
            return Results.Created($"/api/students/{result.Id}", ApiResponse<StudentResponse>.Ok(result));
        });

        group.MapPut("/{id:guid}", async (Guid id, StudentUpdateRequest request, IStudentService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return Results.Ok(ApiResponse<StudentResponse>.Ok(result));
        });

        group.MapDelete("/{id:guid}", async (Guid id, IStudentService service, CancellationToken ct) =>
        {
            await service.DeleteAsync(id, ct);
            return Results.NoContent();
        });

        return app;
    }
}
