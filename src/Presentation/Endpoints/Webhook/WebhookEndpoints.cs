using System.Text.Json;
using StudentApi.Api.Models;
using StudentApi.Application.Context;
using StudentApi.Application.DTOs.Webhooks;
using StudentApi.Application.Repositories;
using StudentApi.Domain.ErrorCodes;
using StudentApi.Domain.Exceptions;
using StudentApi.Domain.Models.Webhooks;

namespace StudentApi.Api.Endpoints;

public static class WebhookEndpoints
{
    public static WebApplication MapWebhookEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/webhooks").WithTags("Webhooks").RequireAuthorization();

        group.MapGet("/", async (
            IWebhookSubscriptionRepository repo,
            ITenantContext tenant,
            CancellationToken ct) =>
        {
            var subs = await repo.GetAllByTenantAsync(tenant.TenantId, ct);
            var result = subs.Select(ToResponse).ToList();
            return Results.Ok(ApiResponse<IReadOnlyCollection<WebhookSubscriptionResponse>>.Ok(result));
        });

        group.MapPost("/", async (
            WebhookSubscriptionCreateRequest request,
            IWebhookSubscriptionRepository repo,
            ITenantContext tenant,
            CancellationToken ct) =>
        {
            if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
                throw new BadRequestException(
                    StudentApiErrorCodes.General.InvalidRequest, "Url is not a valid absolute URI.");

            var entity = new WebhookSubscriptionEntity
            {
                Id        = Guid.NewGuid(),
                TenantId  = tenant.TenantId,
                Url       = request.Url,
                Events    = JsonSerializer.Serialize(request.Events),
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
            };

            await repo.AddAsync(entity, ct);

            return Results.Created(
                $"/api/webhooks/{entity.Id}",
                ApiResponse<WebhookSubscriptionResponse>.Ok(ToResponse(entity)));
        });

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IWebhookSubscriptionRepository repo,
            ITenantContext tenant,
            CancellationToken ct) =>
        {
            var entity = await repo.GetByIdAsync(id, tenant.TenantId, ct)
                ?? throw new NotFoundException(
                    StudentApiErrorCodes.General.InvalidRequest, $"Webhook subscription '{id}' was not found.");

            await repo.DeleteAsync(entity, ct);
            return Results.Ok(ApiResponse<string>.Ok("Webhook subscription deleted."));
        });

        return app;
    }

    private static WebhookSubscriptionResponse ToResponse(WebhookSubscriptionEntity e)
    {
        var events = JsonSerializer.Deserialize<List<string>>(e.Events) ?? [];
        return new WebhookSubscriptionResponse(e.Id, e.TenantId, e.Url, events, e.CreatedAt);
    }
}
