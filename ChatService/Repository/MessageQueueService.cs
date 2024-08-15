using ChatService.Interface;
using ChatService.Models;
using System.Text.Json;

namespace ChatService.Repository
{
    public class UserMessageRepository: IMessageQueue
    {
        private readonly HttpClient _cacheClient;
        private readonly ILogger<UserMessageRepository> _logger;

        public UserMessageRepository(IHttpClientFactory clientFactory, ILogger<UserMessageRepository> logger)
        {
            _cacheClient = clientFactory.CreateClient("CacheService");
            _logger = logger;
        }

        public async Task<bool> AddToCacheQueue(CacheMessage cacheMessage)
        {
            var Success=false;
            try
            {
                var saveCacheMessages = JsonContent.Create(cacheMessage);
                var response = await _cacheClient.PostAsync("/api/cache/saveMessages", saveCacheMessages);
                var responseData = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<Response<bool>>(responseData);
                Success=data.success;
                _logger.LogInformation("Successfully cached the message with connection ID: {ConnectionId}", data.success);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to cache the messages", ex.Message);
                return Success;
            }

            return Success;
        }
    }
}
