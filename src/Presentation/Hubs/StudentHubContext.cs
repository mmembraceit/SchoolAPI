using Microsoft.AspNetCore.SignalR;
using StudentApi.Application.Hubs;
using StudentApi.Application.Messaging.Events;

namespace StudentApi.Api.Hubs;

internal sealed class StudentHubContext(IHubContext<StudentHub> hubContext) : IStudentHubContext
{
    public Task NotifyStudentCreatedAsync(StudentCreatedEvent @event, CancellationToken cancellationToken)
        => hubContext.Clients
            .Group(@event.TenantId.ToString())
            .SendAsync("StudentCreated", @event, cancellationToken);

    public Task NotifyStudentUpdatedAsync(StudentUpdatedEvent @event, CancellationToken cancellationToken)
        => hubContext.Clients
            .Group(@event.TenantId.ToString())
            .SendAsync("StudentUpdated", @event, cancellationToken);

    public Task NotifyStudentDeletedAsync(StudentDeletedEvent @event, CancellationToken cancellationToken)
        => hubContext.Clients
            .Group(@event.TenantId.ToString())
            .SendAsync("StudentDeleted", @event, cancellationToken);
}
