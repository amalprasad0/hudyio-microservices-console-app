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
            var mobileNumber = Context.GetHttpContext().Request.Query["mobileNumber"].ToString();

            if (!string.IsNullOrEmpty(mobileNumber))
            {
                _connections[mobileNumber] = Context.ConnectionId;
                _logger.LogInformation("User connected: {MobileNumber} with ConnectionId: {ConnectionId}", mobileNumber, Context.ConnectionId);

                var saveConnectionId = new SaveConnection
                {
                    MobileNumber = mobileNumber,
                    ConnectionId = Context.ConnectionId
                };

                try
                {
                    var saveConnection = JsonContent.Create(saveConnectionId);
                    var response = await _client.PostAsync("/api/User/saveconnectionid", saveConnection);

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("Successfully saved connection ID for {MobileNumber}", mobileNumber);
                    }
                    else
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        _logger.LogWarning("Failed to save connection ID. Status Code: {StatusCode}, Response: {ResponseBody}", response.StatusCode, responseBody);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while saving connection ID for {MobileNumber}", mobileNumber);
                }

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
            var mobileNumber = _connections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;

            if (!string.IsNullOrEmpty(mobileNumber))
            {
                _connections.Remove(mobileNumber);
                _logger.LogInformation("User disconnected: {MobileNumber} with ConnectionId: {ConnectionId}", mobileNumber, Context.ConnectionId);
                await Clients.All.SendAsync("UserDisconnected", mobileNumber);
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
