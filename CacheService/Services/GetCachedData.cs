using CacheService.Helper;
using CacheService.Interfaces;
using StackExchange.Redis;
using CacheService.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
namespace CacheService.CacheMigration
{
    public class GetCachedData:IGetCachedData
    {
        private IDatabase _db;
        private readonly ApiService _apiService;
        private HttpClient _userClient;
        private readonly IQueueCache _queueCache;
        public GetCachedData(ApiService apiService,IQueueCache queueCache,IHttpClientFactory clientFactory)
        {
            _db = ConnectionHelper.Connection.GetDatabase();
            _apiService = apiService;
            _queueCache = queueCache;
            _userClient=clientFactory.CreateClient("UserService");
        }
        public async Task<List<int>> GetCachedUserId()
        {
            var response = await _apiService.ExecuteGet<List<int>>(_userClient, "/api/User/getCachedUserIds");
            return response.data;
        }
        public async Task<List<string>> GetCachedMessageId()
        {
            var cachedMessageId = new List<string>();
            var userIds = await GetCachedUserId();
            if (userIds != null && userIds.Count>0)
            {
                foreach (var userId in userIds)
                {
                    var dequeuedMessage = _queueCache.GetAllMessages<UserMessage>($"msg" + userId);
                    if(dequeuedMessage.data != null)
                    {
                        foreach (var message in dequeuedMessage.data)
                        {
                            if (message.MessageId != null)
                                cachedMessageId.Add(message.MessageId);
                        }
                    }
                    
                }
            }
            return cachedMessageId;
        }
    }
}
