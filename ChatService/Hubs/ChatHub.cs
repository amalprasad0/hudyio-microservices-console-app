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
        private readonly HttpClient _userClient;
        private readonly HttpClient _cacheClient;

        public ChatHub(ILogger<ChatHub> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _userClient = clientFactory.CreateClient("UserService");
            _cacheClient = clientFactory.CreateClient("CacheService");

        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.GetHttpContext().Request.Query["userId"].ToString();

            if (!string.IsNullOrEmpty(userId))
            {
                _connections[userId] = Context.ConnectionId;
                _logger.LogInformation("User connected: {MobileNumber} with ConnectionId: {ConnectionId}", userId, Context.ConnectionId);

                var userConnection = new CacheRecord
                {
                    Value = Context.ConnectionId,
                    Key = userId,
                    Expiration = DateTime.UtcNow.AddDays(1)
                };

                try
                {
                    try
                    {
                        var saveConnection = JsonContent.Create(userConnection);
                        //var response = await _userClient.PostAsync("/api/User/saveconnectionid", saveConnection);
                        var response = await _cacheClient.PostAsync("/api/Cache/set", saveConnection);

                        if (response.IsSuccessStatusCode)
                        {
                            _logger.LogInformation("Successfully saved connection ID for {MobileNumber} | Status Code: {StatusCode}", userId, response.StatusCode);
                        }
                        else
                        {
                            var responseBody = await response.Content.ReadAsStringAsync();
                            _logger.LogWarning("Failed to save connection ID. Status Code: {StatusCode}, Response: {ResponseBody}", response.StatusCode, responseBody);
                        }
                    }
                    catch (Exception apiEx)
                    {
                        _logger.LogError(apiEx, "Error while calling the API to save connection ID for {MobileNumber}", userId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "General error while handling user connection for {MobileNumber}", userId);
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
            try
            {
                var userId = _connections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;

                if (!string.IsNullOrEmpty(userId))
                {
                    try
                    {
                        var response2 = _cacheClient.GetAsync($"/api/Cache/{userId}");

                        var response = await _cacheClient.DeleteAsync($"/api/Cache/{userId}");
                        if (response.IsSuccessStatusCode)
                        {
                            _logger.LogInformation("Successfully removed connection ID for {userId}: {Response}", userId, response2);
                        }
                        else
                        {
                            _logger.LogWarning("Error while removing connection ID for {userId}: {Response}", userId, response);
                        }
                    }
                    catch (Exception apiEx)
                    {
                        _logger.LogError(apiEx, "An error occurred while calling the API to remove the connection ID for {userId}", userId);
                    }

                    _logger.LogInformation("User disconnected: {userId} with ConnectionId: {ConnectionId}", userId, Context.ConnectionId);
                    await Clients.All.SendAsync("UserDisconnected", userId);
                }

                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling user disconnection. ConnectionId: {ConnectionId}", Context.ConnectionId);
            }
        }


        public async Task SendMessage(string userId, string message)
        {
            try
            {
                var response = await _cacheClient.GetAsync($"/api/Cache/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();

                    var connectionId = JsonSerializer.Deserialize<string>(responseData);

                    if (connectionId != null && !string.IsNullOrEmpty(connectionId))
                    {
                        await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
                        _logger.LogInformation("Sent message to {userId}: {Message}", userId, message);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to send message. Connection ID not found for userId: {userId}", userId);
                    }
                }
                else
                {
                    _logger.LogWarning("Failed to retrieve connection ID from cache service. Status Code: {StatusCode}, UserId: {userId}", response.StatusCode, userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending message to {userId}. Message: {Message}", userId, message);
            }
        }


    }
}
