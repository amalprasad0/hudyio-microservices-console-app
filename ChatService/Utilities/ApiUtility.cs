using ChatService.Models;
using System.Net.Http;
using System.Text.Json;
namespace ChatService.Utilities
{
    public class ApiUtility
    {
        private readonly HttpClient _cacheClient;
        public ApiUtility(IHttpClientFactory httpClientFactory)
        {
            _cacheClient = httpClientFactory.CreateClient("CacheService");
        }
        public async Task<Response<T>> PostToApiAsync<T>(string endpoint, object payload)
        {
            var response = new Response<T>();

            try
            {
                var content = JsonContent.Create(payload);
                var apiResponse = await _cacheClient.PostAsync(endpoint, content);

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
        public async Task<Response<T>> GetFromApiAsync<T>(string endpoint)
        {
            var response = new Response<T>();

            try
            {
                var apiResponse = await _cacheClient.GetAsync(endpoint);

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


    }
}
