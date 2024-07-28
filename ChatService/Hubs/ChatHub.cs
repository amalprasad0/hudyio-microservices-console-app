using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ChatService.Hubs
{
    public sealed class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }

        public async Task SendMessage(string message)
        {
            _logger.LogInformation("SendMessage called with message: {Message}", message);
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("User connected: {ConnectionId}", Context.ConnectionId);
            await Clients.All.SendAsync("UserConnected", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation("User disconnected: {ConnectionId}", Context.ConnectionId);
            await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
