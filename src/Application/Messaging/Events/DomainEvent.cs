namespace StudentApi.Application.Messaging.Events;

/// <summary>
/// Base record for all domain events published to Azure Service Bus.
/// The <see cref="EventType"/> property is used as the message Subject
/// so topic subscriptions can filter by event type.
/// </summary>
public abstract record DomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
    public abstract string EventType { get; }
}
