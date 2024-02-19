using Microsoft.AspNetCore.SignalR;

namespace ChartsServer.Hubs;

public class SaleHub : Hub
{
    public async Task SendMessageAsync()
    {
        await Clients.All.SendAsync("receiveMessage", "Merhaba");
    }
}
