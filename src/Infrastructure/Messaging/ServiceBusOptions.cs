namespace StudentApi.Infrastructure.Messaging;

public sealed class ServiceBusOptions
{
    public const string SectionName = "ServiceBus";

    /// <summary>
    /// Azure Service Bus connection string.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Topic name for student domain events 
    /// </summary>
    public string StudentsTopic { get; set; } = "students";

    /// <summary>
    /// Subscription name this service instance reads from 
    /// </summary>
    public string StudentsSubscription { get; set; } = "school-api";
}
