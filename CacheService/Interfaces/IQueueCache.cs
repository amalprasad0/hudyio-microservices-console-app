using CacheService.Models;

namespace CacheService.Interfaces
{
    public interface IQueueCache
    {
        Response<bool> EnqueueMessage<T>(string key, List<T> Messages);
        Response<T> DequeueMessage<T>(string key);
    }
}
