using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StudentApi.Application.Messaging.Events;

namespace StudentApi.Infrastructure.Messaging;

/// <summary>
/// Background service that processes student domain events arriving on
/// the configured Azure Service Bus topic subscription.
/// </summary>
public sealed class StudentEventProcessor : BackgroundService, IAsyncDisposable
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusOptions _options;
    private readonly ILogger<StudentEventProcessor> _logger;

    private ServiceBusProcessor? _processor;

    public StudentEventProcessor(
        ServiceBusClient client,
        IOptions<ServiceBusOptions> options,
        ILogger<StudentEventProcessor> logger)
    {
        _client  = client;
        _options = options.Value;
        _logger  = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor = _client.CreateProcessor(
            _options.StudentsTopic,
            _options.StudentsSubscription,
            new ServiceBusProcessorOptions
            {
                AutoCompleteMessages   = false,
                MaxConcurrentCalls     = 4,
                ReceiveMode            = ServiceBusReceiveMode.PeekLock
            });

        _processor.ProcessMessageAsync += OnMessageAsync;
        _processor.ProcessErrorAsync   += OnErrorAsync;

        await _processor.StartProcessingAsync(stoppingToken);

        _logger.LogInformation(
            "StudentEventProcessor started. Listening on topic '{Topic}', subscription '{Subscription}'",
            _options.StudentsTopic, _options.StudentsSubscription);

        // Keep alive until the host requests shutdown.
        await Task.Delay(Timeout.Infinite, stoppingToken).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);

        await _processor.StopProcessingAsync(CancellationToken.None);
        _logger.LogInformation("StudentEventProcessor stopped.");
    }

    private async Task OnMessageAsync(ProcessMessageEventArgs args)
    {
        var subject = args.Message.Subject;

        _logger.LogInformation(
            "Received message: Subject={Subject} MessageId={MessageId}",
            subject, args.Message.MessageId);

        try
        {
            await HandleAsync(subject, args.Message, args.CancellationToken);
            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to process message: Subject={Subject} MessageId={MessageId}. Abandoning.",
                subject, args.Message.MessageId);

            await args.AbandonMessageAsync(args.Message, cancellationToken: args.CancellationToken);
        }
    }

    private Task OnErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception,
            "Service Bus processor error. Source={Source} EntityPath={EntityPath}",
            args.ErrorSource, args.EntityPath);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Dispatches to a typed handler based on the message Subject.
    /// </summary>
    private Task HandleAsync(string subject, ServiceBusReceivedMessage message, CancellationToken ct)
    {
        var body = message.Body.ToArray();

        return subject switch
        {
            "student.created" => HandleStudentCreatedAsync(
                JsonSerializer.Deserialize<StudentCreatedEvent>(body)!, ct),

            "student.updated" => HandleStudentUpdatedAsync(
                JsonSerializer.Deserialize<StudentUpdatedEvent>(body)!, ct),

            "student.deleted" => HandleStudentDeletedAsync(
                JsonSerializer.Deserialize<StudentDeletedEvent>(body)!, ct),

            _ => HandleUnknownAsync(subject, ct)
        };
    }


    // Handlers — replace the log statements with actual business logic.
    private Task HandleStudentCreatedAsync(StudentCreatedEvent @event, CancellationToken ct)
    {
        _logger.LogInformation(
            "Student created — Id={StudentId} TenantId={TenantId} Name={Name}",
            @event.StudentId, @event.TenantId, @event.Name);

        return Task.CompletedTask;
    }

    private Task HandleStudentUpdatedAsync(StudentUpdatedEvent @event, CancellationToken ct)
    {
        _logger.LogInformation(
            "Student updated — Id={StudentId} TenantId={TenantId} Name={Name}",
            @event.StudentId, @event.TenantId, @event.Name);

        return Task.CompletedTask;
    }

    private Task HandleStudentDeletedAsync(StudentDeletedEvent @event, CancellationToken ct)
    {
        _logger.LogInformation(
            "Student deleted — Id={StudentId} TenantId={TenantId}",
            @event.StudentId, @event.TenantId);

        return Task.CompletedTask;
    }

    private Task HandleUnknownAsync(string subject, CancellationToken ct)
    {
        _logger.LogWarning("Received unknown event subject: '{Subject}'. Skipping.", subject);
        return Task.CompletedTask;
    }

    public new async ValueTask DisposeAsync()
    {
        if (_processor is not null)
            await _processor.DisposeAsync();

        await _client.DisposeAsync();
        base.Dispose();
    }
}
