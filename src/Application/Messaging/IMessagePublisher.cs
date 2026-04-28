namespace StudentApi.Application.Messaging;

/// <summary>
/// Abstraction for publishing domain events to a message broker.
/// Implementations should be registered as scoped services.
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    /// Publishes a domain event to the configured topic/queue.
    /// </summary>
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken) where T : Events.DomainEvent;
}
