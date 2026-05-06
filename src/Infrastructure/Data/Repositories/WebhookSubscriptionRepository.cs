using Microsoft.EntityFrameworkCore;
using StudentApi.Application.Repositories;
using StudentApi.Domain.Models.Webhooks;
using StudentApi.Infrastructure.Data;

namespace StudentApi.Infrastructure.Repositories;

public sealed class WebhookSubscriptionRepository(StudentApiDbContext dbContext) : IWebhookSubscriptionRepository
{
    public async Task<IReadOnlyList<WebhookSubscriptionEntity>> GetByTenantAndEventAsync(
        Guid tenantId, string eventType, CancellationToken cancellationToken)
    {
        return await dbContext.WebhookSubscriptions
            .AsNoTracking()
            .Where(w => w.TenantId == tenantId && w.Events.Contains(eventType))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WebhookSubscriptionEntity>> GetAllByTenantAsync(
        Guid tenantId, CancellationToken cancellationToken)
    {
        return await dbContext.WebhookSubscriptions
            .AsNoTracking()
            .Where(w => w.TenantId == tenantId)
            .ToListAsync(cancellationToken);
    }

    public async Task<WebhookSubscriptionEntity?> GetByIdAsync(
        Guid id, Guid tenantId, CancellationToken cancellationToken)
    {
        return await dbContext.WebhookSubscriptions
            .FirstOrDefaultAsync(w => w.Id == id && w.TenantId == tenantId, cancellationToken);
    }

    public async Task AddAsync(WebhookSubscriptionEntity subscription, CancellationToken cancellationToken)
    {
        await dbContext.WebhookSubscriptions.AddAsync(subscription, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(WebhookSubscriptionEntity subscription, CancellationToken cancellationToken)
    {
        subscription.MarkAsDeleted();
        dbContext.WebhookSubscriptions.Update(subscription);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
