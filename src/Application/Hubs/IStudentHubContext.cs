using StudentApi.Application.Messaging.Events;

namespace StudentApi.Application.Hubs;

public interface IStudentHubContext
{
    Task NotifyStudentCreatedAsync(StudentCreatedEvent @event, CancellationToken cancellationToken);
    Task NotifyStudentUpdatedAsync(StudentUpdatedEvent @event, CancellationToken cancellationToken);
    Task NotifyStudentDeletedAsync(StudentDeletedEvent @event, CancellationToken cancellationToken);
}
