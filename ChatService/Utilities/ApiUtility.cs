using ChatService.Models;
using System.Net.Http;
using System.Text.Json;

namespace ChatService.Utilities
{
    public class ApiUtility
    {
        private readonly HttpClient _cacheClient;
        private readonly HttpClient _messageClient;

        public ApiUtility(IHttpClientFactory httpClientFactory)
        {
            _cacheClient = httpClientFactory.CreateClient("CacheService");
            _messageClient = httpClientFactory.CreateClient("MessageService");
        }

        private async Task<Response<T>> ExecuteApiRequest<T>(Func<Task<HttpResponseMessage>> apiCall)
        {
            var response = new Response<T>();
            try
            {
                var apiResponse = await apiCall();
                if (apiResponse.IsSuccessStatusCode)
                {
                    var responseData = await apiResponse.Content.ReadAsStringAsync();
                    response = JsonSerializer.Deserialize<Response<T>>(responseData);
                    response.success = true;
                }
                else
                {
                    response.success = false;
                    response.errorMessage = $"API request failed with status code: {apiResponse.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                response.success = false;
                response.errorMessage = $"An error occurred while calling the API: {ex.Message}";
            }

            return response;
        }

        public Task<Response<T>> PostToApiAsync<T>(string endpoint, object payload) =>
            ExecuteApiRequest<T>(() => _cacheClient.PostAsync(endpoint, JsonContent.Create(payload)));

        public Task<Response<T>> GetFromApiAsync<T>(string endpoint) =>
            ExecuteApiRequest<T>(() => _cacheClient.GetAsync(endpoint));

        public Task<Response<T>> MessagePostApi<T>(string endpoint, object payload) =>
            ExecuteApiRequest<T>(() => _messageClient.PostAsync(endpoint, JsonContent.Create(payload)));
        public Task<Response<T>> MessageGetApi<T>(string endpoint) =>
           ExecuteApiRequest<T>(() => _messageClient.GetAsync(endpoint));
    }
}
