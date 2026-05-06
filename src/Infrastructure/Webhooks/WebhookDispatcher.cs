using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using StudentApi.Application.Context;
using StudentApi.Application.Messaging.Events;
using StudentApi.Application.Repositories;
using StudentApi.Application.Webhooks;

namespace StudentApi.Infrastructure.Webhooks;

public sealed class WebhookDispatcher(
    IWebhookSubscriptionRepository repository,
    ITenantContext tenantContext,
    HttpClient httpClient,
    ILogger<WebhookDispatcher> logger) : IWebhookDispatcher
{
    public async Task DispatchAsync<T>(T @event, CancellationToken cancellationToken) where T : DomainEvent
    {
        var subscriptions = await repository.GetByTenantAndEventAsync(
            tenantContext.TenantId, @event.EventType, cancellationToken);

        if (subscriptions.Count == 0)
            return;

        var payload = new WebhookPayload(
            EventId:    @event.EventId,
            EventType:  @event.EventType,
            OccurredAt: @event.OccurredAt,
            Data:       @event);

        var tasks = subscriptions.Select(sub => SendAsync(sub.Url, payload, cancellationToken));
        await Task.WhenAll(tasks);
    }

    private async Task SendAsync(string url, WebhookPayload payload, CancellationToken cancellationToken)
    {
        try
        {
            using var response = await httpClient.PostAsJsonAsync(url, payload, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Webhook delivery failed. Url={Url} StatusCode={StatusCode} EventType={EventType}",
                    url, (int)response.StatusCode, payload.EventType);
            }
            else
            {
                logger.LogInformation(
                    "Webhook delivered. Url={Url} EventType={EventType}",
                    url, payload.EventType);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Webhook delivery error. Url={Url} EventType={EventType}",
                url, payload.EventType);
        }
    }
}

internal sealed record WebhookPayload(
    Guid EventId,
    string EventType,
    DateTimeOffset OccurredAt,
    object Data);
