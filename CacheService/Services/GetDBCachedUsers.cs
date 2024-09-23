using CacheService.Helper;
using CacheService.Models;
using CacheService.Interfaces;
namespace CacheService.Services
{
    public class GetDBCachedUsers:IGetDBCacheUsercs
    {
        private HttpClient _msgMngClient;
        private ApiService _apiService;
        private IQueueCache _queueCache;
        public GetDBCachedUsers( ApiService apiService, IHttpClientFactory clientFactory, IQueueCache queueCache)

        {
            
            _msgMngClient = clientFactory.CreateClient("MessageMngService");
            _apiService = apiService;
            _queueCache = queueCache;
        }
        public async Task<List<DBCachedUserIds>?> GetDBCachedMessageIds()
        {
            var response = await _apiService.ExecuteGet<List<DBCachedUserIds>>(_msgMngClient, "api/UserMessage/GetDBCachedUserIds");
            return response?.data;
        }
        public async Task<Response<bool>> RemoveDbCachedMSg()
        {
            Response<bool> response = new Response<bool>();
            var isSuccess=false;
            try
            {
                var dBCachedUserIds = await GetDBCachedMessageIds();
                foreach (var item in dBCachedUserIds) {
                    foreach (var cacheId in item.cacheIds) {
                        Response<bool> isRemoved= _queueCache.DequeueMessageById("msg"+item.userId, cacheId);
                        isSuccess=isRemoved.data;

                    }
                }
                response.data = isSuccess;  
                response.success = true;
            }
            
            catch (Exception ex) { 
                response.errorMessage = ex.Message;
                response.success = false;
            }
            return response;
        }
        
    }
}
