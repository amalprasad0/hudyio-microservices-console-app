using ChatService.Models;

namespace ChatService.Interface
{
    public interface IMessageService
    {
        Task<bool> StoreMessage(StoreUserMessage message);
    }
}
