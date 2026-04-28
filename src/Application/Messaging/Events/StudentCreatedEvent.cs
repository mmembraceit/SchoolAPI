namespace StudentApi.Application.Messaging.Events;

public sealed record StudentCreatedEvent(
    Guid StudentId,
    Guid TenantId,
    string Name,
    DateOnly DateOfBirth,
    DateTimeOffset CreatedAt) : DomainEvent
{
    public override string EventType => "student.created";
}
