using StudentApi.Domain.Models.Webhooks;

namespace StudentApi.Application.Repositories;

public interface IWebhookSubscriptionRepository
{
    Task<IReadOnlyList<WebhookSubscriptionEntity>> GetByTenantAndEventAsync(
        Guid tenantId, string eventType, CancellationToken cancellationToken);

    Task<IReadOnlyList<WebhookSubscriptionEntity>> GetAllByTenantAsync(
        Guid tenantId, CancellationToken cancellationToken);

    Task<WebhookSubscriptionEntity?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken);

    Task AddAsync(WebhookSubscriptionEntity subscription, CancellationToken cancellationToken);

    Task DeleteAsync(WebhookSubscriptionEntity subscription, CancellationToken cancellationToken);
}
