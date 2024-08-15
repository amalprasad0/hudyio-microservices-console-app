using ChatService.Models;

namespace ChatService.Interface
{
    public interface IMessageQueue
    {
        Task<bool> AddToCacheQueue(CacheMessage cacheMessage);
    }
}
