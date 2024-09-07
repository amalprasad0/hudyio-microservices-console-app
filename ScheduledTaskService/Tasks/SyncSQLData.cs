using ScheduledTaskService.Interface;
using ScheduledTaskService.Services;
using System.Net.Http;
using System.Threading.Tasks;

namespace ScheduledTaskService.Tasks
{
    public class SyncSQLData
    {
        private readonly IJobTaskScheduler _taskScheduler;
        private readonly HttpClient _httpClient;

        public SyncSQLData(IJobTaskScheduler taskScheduler, IHttpClientFactory clientFactory)
        {
            _taskScheduler = taskScheduler;
            _httpClient = clientFactory.CreateClient("CacheService");
        }

        public async Task FetchDataFromApiAsync(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Data fetched successfully: " + data);
            }
            else
            {
                Console.WriteLine("Failed to fetch data. Status code: " + response.StatusCode);
            }
        }

        public void ScheduleApiDataFetchJob(string jobId, string endpoint, string cronExpression)
        {
            _taskScheduler.ScheduleRecurringJob(
                jobId,
                () => FetchDataFromApiAsync(endpoint).GetAwaiter().GetResult(),
                cronExpression);
        }
    }
}
