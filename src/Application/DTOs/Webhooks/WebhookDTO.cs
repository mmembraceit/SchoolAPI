namespace StudentApi.Application.DTOs.Webhooks;

public sealed record WebhookSubscriptionResponse(
    Guid Id,
    Guid TenantId,
    string Url,
    IReadOnlyList<string> Events,
    DateTimeOffset CreatedAt);

public sealed record WebhookSubscriptionCreateRequest(
    string Url,
    IReadOnlyList<string> Events);
