using ChatService.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatService.Utilities
{
    public class CacheServiceClient
    {
        private readonly HttpClient _cacheClient;

        public CacheServiceClient(IHttpClientFactory clientFactory)
        {
            _cacheClient = clientFactory.CreateClient("CacheService");
        }

        public async Task<bool> SaveConnectionId(CacheRecord cacheRecord)
        {
            var content = JsonContent.Create(cacheRecord);
            var response = await _cacheClient.PostAsync("/api/Cache/set", content);
            var responseData = await response.Content.ReadAsStringAsync();
            var parsedResponse= JsonSerializer.Deserialize<bool>(responseData);
            return parsedResponse;
        }

        public async Task<bool> RemoveConnectionId(string userId)
        {
            var response = await _cacheClient.DeleteAsync($"/api/Cache/{userId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<string> GetConnectionId(string userId)
        {
            var response = await _cacheClient.GetAsync($"/api/Cache/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<string>(responseData);
            }
            return null;
        }
    }
}
