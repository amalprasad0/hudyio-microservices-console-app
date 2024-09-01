using System.Text.Json;
using ScheduledTaskService.Models;
namespace ScheduledTaskService.Helpers
{
    public class ApiService
    {
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

        public Task<Response<T>> ExecutePost<T>(HttpClient client, string endpoint, object payload) =>
            ExecuteApiRequest<T>(() => client.PostAsync(endpoint, JsonContent.Create(payload)));

        public Task<Response<T>> ExecuteGet<T>(HttpClient client, string endpoint) =>
            ExecuteApiRequest<T>(() => client.GetAsync(endpoint));
    }
}
