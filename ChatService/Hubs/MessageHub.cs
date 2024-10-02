using ChatService.Interface;
using ChatService.Models;
using ChatService.Utilities;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Hubs
{

    public class MessageHub : Hub
    {
        private readonly UserConnectionManager _connectionManager;
        private readonly CacheServiceClient _cacheServiceClient;
        private readonly IMessageQueue _messageQueueService;
        private readonly ILogger<MessageHub> _logger;
        private readonly IMessageService _messageService;
        public MessageHub(UserConnectionManager connectionManager, CacheServiceClient cacheServiceClient, IMessageQueue messageQueueService, ILogger<MessageHub> logger, IMessageService messageService)
        {
            _connectionManager = connectionManager;
            _cacheServiceClient = cacheServiceClient;
            _messageQueueService = messageQueueService;
            _logger = logger;
            _messageService = messageService;
        }
        public override async Task OnConnectedAsync()
        {
            var userId = Context.GetHttpContext().Request.Query["userId"].ToString();

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogError("UserId is Null or Unexcepted Error");
                await Clients.All.SendAsync("User Id is Null");
                return;
            }
            _connectionManager.AddConnection(userId, Context.ConnectionId);

            var userConnection = new CacheRecord
            {
                Value = Context.ConnectionId,
                Key = userId,
                Expiration = DateTime.UtcNow.AddDays(1)
            };

            try
            {
                var isSaved = await _cacheServiceClient.SaveConnectionId(userConnection);
                if (isSaved)
                {
                    await Clients.All.SendAsync("UserConnected", userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Couldn't Stored ConnectionId {ex.Message}");
                return;
            }

           // await _messageQueueService.GetAllQueuedMessages(userId, Context.ConnectionId);
            await _messageQueueService.GetAllDBQueuedMessages(userId, Context.ConnectionId);
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.GetHttpContext().Request.Query["userId"].ToString();
            await _cacheServiceClient.RemoveConnectionId(userId);
            _connectionManager.RemoveConnection(userId);
            await Clients.All.SendAsync("UserDisconnected", userId);
            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(string toUserId, string message)
        {
            var connectionId = await _cacheServiceClient.GetConnectionId(toUserId);
            var userId = Context.GetHttpContext().Request.Query["userId"].ToString();
            try
            {
                var messageId = Guid.NewGuid().ToString();
                var storeUserMessage = new StoreUserMessage
                {
                    cachedMessageId = messageId,
                    messageContent = message,
                    fromUserId = int.Parse(userId),
                    toUserId = int.Parse(toUserId),
                    hasFile = false,
                    fileType = "N/A",
                    fileUrl = "N?A"
                };
                await _messageService.StoreMessage(storeUserMessage);
                if (string.IsNullOrEmpty(connectionId))
                {
                    
                    var userMessage = new CacheMessage
                    {
                        ToUserId = "msg" + toUserId,
                        MessageData = new UserMessage
                        {

                            MessageContent = message,
                            MessageTime = DateTime.Now,
                            SendByUser = Context.GetHttpContext().Request.Query["userId"].ToString(),
                            MessageId = messageId

                        }
                    };

                    await _messageQueueService.AddToCacheQueue(userMessage);
                    await Clients.Caller.SendAsync("ReciecerIsOffline");
                    return;
                }
                
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
               
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexcepted Error while sending Message{ex.Message}");
                await Clients.Caller.SendAsync("Unable to send Message", message);
            }

        }
        public async Task GetDeliveryReport(string userId, string MessageId)
        {
            try
            {
                var response=await _messageQueueService.RemoveCachedMessage(userId, MessageId);
                if(!response)
                {
                    await Clients.Caller.SendAsync($"NotRemoveCachedMessage: {MessageId}");
                    return;
                }
                await Clients.Caller.SendAsync($"RemovedCachedMessage: {MessageId}");
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync($"ErrorOccured: {ex.Message}");
            }
        }
    }
}
