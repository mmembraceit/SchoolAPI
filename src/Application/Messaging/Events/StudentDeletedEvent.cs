namespace StudentApi.Application.Messaging.Events;

public sealed record StudentDeletedEvent(
    Guid StudentId,
    Guid TenantId,
    DateTimeOffset DeletedAt) : DomainEvent
{
    public override string EventType => "student.deleted";
}
