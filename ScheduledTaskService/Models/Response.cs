namespace ScheduledTaskService.Models
{
    public class Response<T>
    {
        public bool success { get; set; }
        public T data { get; set; }
        public string errorMessage { get; set; }
    }
}
