using Microsoft.Extensions.Logging;
using StudentApi.Application.Messaging;
using StudentApi.Application.Messaging.Events;

namespace StudentApi.Infrastructure.Messaging;

/// <summary>
/// No-op publisher used when Azure Service Bus is not configured (e.g. local dev without Azure).
/// Logs a warning so developers are aware that events are not being published.
/// </summary>
internal sealed class NullMessagePublisher : IMessagePublisher
{
    private readonly ILogger<NullMessagePublisher> _logger;

    public NullMessagePublisher(ILogger<NullMessagePublisher> logger)
        => _logger = logger;

    public Task PublishAsync<T>(T @event, CancellationToken cancellationToken)
        where T : DomainEvent
    {
        _logger.LogWarning(
            "Service Bus is not configured. Event '{EventType}' (EventId={EventId}) was NOT published.",
            @event.EventType, @event.EventId);

        return Task.CompletedTask;
    }
}
