using CacheService.Models;

namespace CacheService.Interfaces
{
    public interface IQueueCache
    {
        Response<bool> EnqueueMessage(SaveRecentMessages saveRecentMessages);
        Response<List<T>> GetAllMessages<T>(string key);
        Response<bool> DequeueMessageById(string userId, string messageId);
    }
}
