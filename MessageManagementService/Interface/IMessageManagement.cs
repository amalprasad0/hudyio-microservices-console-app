using MessageManagementService.Models;
namespace MessageManagementService.Interface
{
    public interface IMessageManagement
    {
        Task<Response<bool>> StoreUserMessage(UserMessage userMessage);
    }
}
