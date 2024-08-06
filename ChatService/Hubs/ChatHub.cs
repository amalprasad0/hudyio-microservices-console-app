using ChatService.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ChatService.Hubs
{
    public sealed class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;
        private static readonly Dictionary<string, string> _connections = new Dictionary<string, string>();
        private readonly HttpClient _client;


        public ChatHub(ILogger<ChatHub> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _client = clientFactory.CreateClient("ServiceClient");

        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.GetHttpContext().Request.Query["userId"].ToString();

            if (!string.IsNullOrEmpty(userId))
            {
                _connections[userId] = Context.ConnectionId;
                _logger.LogInformation("User connected: {MobileNumber} with ConnectionId: {ConnectionId}", userId, Context.ConnectionId);

                var saveConnectionId = new SaveConnection
                {
                    userId = userId,
                    ConnectionId = Context.ConnectionId
                };

                try
                {
                    var saveConnection = JsonContent.Create(saveConnectionId);
                    var response = await _client.PostAsync("/api/User/saveconnectionid", saveConnection);

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("Successfully saved connection ID for {MobileNumber}", userId);
                    }
                    else
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        _logger.LogWarning("Failed to save connection ID. Status Code: {StatusCode}, Response: {ResponseBody}", response.StatusCode, responseBody);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while saving connection ID for {MobileNumber}", userId);
                }

                await Clients.All.SendAsync("UserConnected", userId);
            }
            else
            {
                _logger.LogWarning("Connection attempt without mobile number");
            }

            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = _connections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;

            if (!string.IsNullOrEmpty(userId))
            {
                var removeuserId = JsonContent.Create(userId);
                var response = await _client.PostAsync("/api/User/removeconnectionid", removeuserId);
                _connections.Remove(userId);
                _logger.LogInformation("User disconnected: {MobileNumber} with ConnectionId: {ConnectionId}", userId, Context.ConnectionId);
                await Clients.All.SendAsync("UserDisconnected", userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string mobileNumber, string message)
        {
            if (_connections.TryGetValue(mobileNumber, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
                _logger.LogInformation("Sent message to {MobileNumber}: {Message}", mobileNumber, message);
            }
            else
            {
                _logger.LogWarning("Failed to send message. Mobile number not connected: {MobileNumber}", mobileNumber);
            }
        }
    }
}
