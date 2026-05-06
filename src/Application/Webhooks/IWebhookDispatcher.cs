using StudentApi.Application.Messaging.Events;

namespace StudentApi.Application.Webhooks;

public interface IWebhookDispatcher
{
    Task DispatchAsync<T>(T @event, CancellationToken cancellationToken) where T : DomainEvent;
}
