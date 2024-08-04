using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ChatService.Hubs
{
    public sealed class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;
        private static readonly Dictionary<string, string> _connections = new Dictionary<string, string>();

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
            var mobileNumber = Context.GetHttpContext().Request.Query["mobileNumber"].ToString();

            if (!string.IsNullOrEmpty(mobileNumber))
            {
                _connections[mobileNumber] = Context.ConnectionId;
                _logger.LogInformation("User connected: {MobileNumber} with ConnectionId: {ConnectionId}", mobileNumber, Context.ConnectionId);
                await Clients.All.SendAsync("UserConnected", mobileNumber);
            }
            else
            {
                _logger.LogWarning("Connection attempt without mobile number");
            }

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
