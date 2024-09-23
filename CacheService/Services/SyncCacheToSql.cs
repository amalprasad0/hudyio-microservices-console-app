using CacheService.Helper;
using CacheService.Interfaces;
using System.Net.Http;
namespace CacheService.Services
{
    public class SyncCacheToSql:ISyncDataToSql
    {
        private readonly IGetCachedData _getCachedData;
        private HttpClient _msgMngClient;
        private ApiService _apiService;
        public SyncCacheToSql(IGetCachedData getCachedData,ApiService apiService, IHttpClientFactory clientFactory)
       { 
            _getCachedData = getCachedData;
            _msgMngClient = clientFactory.CreateClient("MessageMngService");
            _apiService = apiService;
        }
        public bool SyncDataToSql()
        {
            var cachedMessageIds = _getCachedData.GetCachedMessageId().Result;
            if (cachedMessageIds != null)
            {
                var response = _apiService.ExecutePost<bool>(_msgMngClient, "/api/UserMessage/StoreCachedMessage", cachedMessageIds).Result;
                if (response != null)
                { 
                    return response.data;
                }
            }
            return false;
        }

    }
}
