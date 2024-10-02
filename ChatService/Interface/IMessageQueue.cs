using ChatService.Models;

namespace ChatService.Interface
{
    public interface IMessageQueue
    {
        Task<bool> AddToCacheQueue(CacheMessage cacheMessage);
        Task GetAllQueuedMessages(string userId, string connectionId);
        Task<bool> RemoveCachedMessage(string userId, string messageId);
        Task GetAllDBQueuedMessages(string userId, string connectionId);
    }
}
