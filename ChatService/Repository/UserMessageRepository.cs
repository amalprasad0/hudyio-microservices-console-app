using ChatService.Models;

namespace ChatService.Repository
{
    public class UserMessageRepository
    {
        private readonly HttpClient _cacheClient;
        private readonly ILogger<UserMessageRepository> _logger;
        public UserMessageRepository(IHttpClientFactory clientFactory, ILogger<UserMessageRepository> logger)
        {
            _cacheClient = clientFactory.CreateClient("CacheService");
            _logger = logger;
        }
        public bool AddToQueue(CacheMessage cacheMessage)
        {
            try {
                var saveCacheMessages = JsonContent.Create(cacheMessage);
                var response=_cacheClient.PostAsync("/api/cache/saveMessages", saveCacheMessages);

            } catch (Exception ex) {
                return false;
               _logger.LogError("Unable to cache the the messages",ex); 
            }
            
            return true;
        }

    }
}
