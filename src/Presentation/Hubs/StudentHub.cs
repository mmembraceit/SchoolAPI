using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace StudentApi.Api.Hubs;

[Authorize]
public sealed class StudentHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var tenantId = Context.User?.FindFirst("tenantId")?.Value;
        if (tenantId is not null)
            await Groups.AddToGroupAsync(Context.ConnectionId, tenantId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var tenantId = Context.User?.FindFirst("tenantId")?.Value;
        if (tenantId is not null)
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, tenantId);

        await base.OnDisconnectedAsync(exception);
    }
}
