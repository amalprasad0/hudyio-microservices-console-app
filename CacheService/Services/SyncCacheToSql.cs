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
        public async Task<bool> SyncDataToSql()
        {
           var cachedMessageIds = await _getCachedData.GetCachedMessageId();
            if (cachedMessageIds != null) {
                var response = await _apiService.ExecutePost<bool>(_msgMngClient,"/api/UserMessage/StoreCachedMessage", cachedMessageIds);
                if (response != null)
                {
                    return response.data;
                }
            }
            return false;

        }
    }
}
