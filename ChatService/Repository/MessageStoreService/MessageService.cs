using ChatService.Interface;
using ChatService.Utilities;
using ChatService.Models;

namespace ChatService.Repository.MessageStoreService
{
    public class MessageService : IMessageService
    {
        private readonly ApiUtility _apiUtility;
        public MessageService(ApiUtility apiUtility) {
            _apiUtility = apiUtility;
        }
        public async Task<bool> StoreMessage(StoreUserMessage message)
        {
            try
            {
                var response = await _apiUtility.MessagePostApi<bool>("/api/usermessage/storemessage", message);
                if (response == null) { 
                return false;
                }
                return response.success;
            }
            catch (Exception ex) {
                return false;
            }
        }
    }
}
