namespace StudentApi.Infrastructure.Messaging;

public sealed class ServiceBusOptions
{
    public const string SectionName = "ServiceBus";

    /// <summary>
    /// Azure Service Bus connection string.
    /// For production, prefer using DefaultAzureCredential by setting
    /// FullyQualifiedNamespace instead.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Topic name for student domain events (e.g. "students").
    /// </summary>
    public string StudentsTopic { get; set; } = "students";

    /// <summary>
    /// Subscription name this service instance reads from (e.g. "school-api").
    /// </summary>
    public string StudentsSubscription { get; set; } = "school-api";
}
