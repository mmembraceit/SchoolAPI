using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StudentApi.Application.Messaging;
using StudentApi.Application.Messaging.Events;

namespace StudentApi.Infrastructure.Messaging;

/// <summary>
/// Publishes domain events to an Azure Service Bus topic.
/// Each message carries:
///   - Body:    JSON-serialised event payload
///   - Subject: EventType string (e.g. "student.created") — useful for topic subscription filters
///   - MessageId: EventId.ToString() — enables duplicate detection
///   - ApplicationProperties["tenantId"]: tenant GUID — useful for per-tenant filters
/// </summary>
public sealed class ServiceBusPublisher : IMessagePublisher, IAsyncDisposable
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusOptions _options;
    private readonly ILogger<ServiceBusPublisher> _logger;

    // Senders are thread-safe and can be reused; cache by topic name.
    private readonly Dictionary<string, ServiceBusSender> _senders = new();

    public ServiceBusPublisher(
        ServiceBusClient client,
        IOptions<ServiceBusOptions> options,
        ILogger<ServiceBusPublisher> logger)
    {
        _client  = client;
        _options = options.Value;
        _logger  = logger;
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken)
        where T : DomainEvent
    {
        var topicName = ResolveTopicName(@event);
        var sender    = GetOrCreateSender(topicName);

        var body    = JsonSerializer.SerializeToUtf8Bytes(@event);
        var message = new ServiceBusMessage(body)
        {
            MessageId   = @event.EventId.ToString(),
            Subject     = @event.EventType,
            ContentType = "application/json",
        };

        // Add tenantId as an application property so subscriptions can filter per tenant.
        if (@event is StudentCreatedEvent created)
            message.ApplicationProperties["tenantId"] = created.TenantId.ToString();
        else if (@event is StudentUpdatedEvent updated)
            message.ApplicationProperties["tenantId"] = updated.TenantId.ToString();
        else if (@event is StudentDeletedEvent deleted)
            message.ApplicationProperties["tenantId"] = deleted.TenantId.ToString();

        _logger.LogInformation(
            "Publishing {EventType} (EventId={EventId}) to topic '{Topic}'",
            @event.EventType, @event.EventId, topicName);

        await sender.SendMessageAsync(message, cancellationToken);

        _logger.LogInformation(
            "Published {EventType} (EventId={EventId}) successfully",
            @event.EventType, @event.EventId);
    }

    private string ResolveTopicName(DomainEvent @event) =>
        @event switch
        {
            StudentCreatedEvent or StudentUpdatedEvent or StudentDeletedEvent => _options.StudentsTopic,
            _ => throw new InvalidOperationException($"No topic configured for event type '{@event.EventType}'.")
        };

    private ServiceBusSender GetOrCreateSender(string topicName)
    {
        if (!_senders.TryGetValue(topicName, out var sender))
        {
            sender = _client.CreateSender(topicName);
            _senders[topicName] = sender;
        }
        return sender;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var sender in _senders.Values)
            await sender.DisposeAsync();

        await _client.DisposeAsync();
    }
}
