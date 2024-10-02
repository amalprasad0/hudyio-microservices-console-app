using ChatService.Hubs;
using ChatService.Interface;
using ChatService.Models;
using ChatService.Utilities;
using Microsoft.AspNetCore.SignalR;
using System.Net;
using System.Text.Json;

namespace ChatService.Repository
{
    public class UserMessageRepository : IMessageQueue
    {
        private readonly HttpClient _cacheClient,_messageClient;
        private readonly ILogger<UserMessageRepository> _logger;
        private readonly ApiUtility _apiUtility;
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly CacheServiceClient _cacheServiceClient;

        public UserMessageRepository(IHttpClientFactory clientFactory, ILogger<UserMessageRepository> logger, ApiUtility apiUtility, IHubContext<MessageHub> hubContext, CacheServiceClient cacheServiceClient)
        {
            _cacheClient = clientFactory.CreateClient("CacheService");
            _messageClient=clientFactory.CreateClient("MessageService");
            _logger = logger;
            _apiUtility = apiUtility;
            _hubContext = hubContext;
            _cacheServiceClient = cacheServiceClient;
        }

        public async Task<bool> AddToCacheQueue(CacheMessage cacheMessage)
        {
            var Success = false;
            try
            {
                var saveCacheMessages = JsonContent.Create(cacheMessage);
                var response = await _cacheClient.PostAsync("/api/cache/saveMessages", saveCacheMessages);
                var responseData = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<Response<bool>>(responseData);
                Success = data.success;
                _logger.LogInformation("Successfully cached the message with connection ID: {ConnectionId}", data.success);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to cache the messages", ex.Message);
                return Success;
            }

            return Success;
        }
        public async Task GetAllQueuedMessages(string userId, string connectionId)
        {
            var response = await _apiUtility.GetFromApiAsync<List<RecievedUserMessages>>($"/api/Cache/GetMessages?key=msg{userId}");
            if (!response.success)
            {
                _logger.LogError($"Failed to retrieve messages. Error: {response.errorMessage}");
            }
            if (response.data == null)
            {
                return;
            }

            if (response.data.Count > 0) {
                foreach (var message in response.data)
                {
                    _logger.LogInformation($"{message.messageContent}");
                    await _hubContext.Clients.Client(connectionId).SendAsync("RecievedMessage", message.sendByUser, message.messageContent, message.messageTime, message.messageId ?? "Not Available");
                }
            }
        }
        public async Task<bool> RemoveCachedMessage(string userId, string messageId)
        {
            var messageRemovalData = new CachedMessageRemoval
            {
                userId = "msg" + userId,
                messageId = messageId
            };
             var response=await _cacheServiceClient.GetDeliveryReport(messageRemovalData);
             return response.data;
        }
        public async Task GetAllDBQueuedMessages(string userId, string connectionId)
        {
            try
            {
                var response = await _apiUtility.MessageGetApi<List<RecievedUserMessages>>($"/api/UserMessage/GetCachedMessages?userId={userId}");
                if (!response.success)
                {
                    _logger.LogError($"Failed to retrieve messages. Error: {response.errorMessage}");
                }
                if (response.data == null)
                {
                    return;
                }

                if (response.data.Count > 0)
                {
                    foreach (var message in response.data)
                    {
                        _logger.LogInformation($"{message.messageContent}");
                        await _hubContext.Clients.Client(connectionId).SendAsync("RecievedMessage", message.sendByUser, message.messageContent, message.messageTime, message.messageId ?? "Not Available");
                    }
                }
            }
            catch (Exception ex) 
            { 
              throw(ex);
            }
        }
    }
}
