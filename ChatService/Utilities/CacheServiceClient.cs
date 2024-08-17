﻿using ChatService.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatService.Utilities
{
    public class CacheServiceClient
    {
        private readonly HttpClient _cacheClient;
        private readonly ApiUtility _apiUtility;    

        public CacheServiceClient(IHttpClientFactory clientFactory,ApiUtility apiUtility)
        {
            _cacheClient = clientFactory.CreateClient("CacheService");
            _apiUtility = apiUtility;
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
        public async Task<Response<bool>> GetDeliveryReport(CachedMessageRemoval messageRemoval)
        {
           
            

               var response = await _apiUtility.PostToApiAsync<bool>("/api/cache/Removecachedmessage", messageRemoval);
                if (response.success)
                {
                    return response;
                }
                return null ;
            
        } 
    }
}
