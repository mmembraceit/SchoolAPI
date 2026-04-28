namespace StudentApi.Application.Messaging.Events;

public sealed record StudentUpdatedEvent(
    Guid StudentId,
    Guid TenantId,
    string Name,
    DateOnly DateOfBirth,
    DateTimeOffset UpdatedAt) : DomainEvent
{
    public override string EventType => "student.updated";
}
