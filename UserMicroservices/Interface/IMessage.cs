using UserMicroservices.Models;

namespace UserMicroservices.Interface
{
    public interface IMessage
    {
        Task<Response<bool>> StoreUserMessage(UserMessage userMessage);
    }
}
