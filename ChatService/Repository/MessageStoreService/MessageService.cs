namespace ChatService.Repository.MessageStoreService
{
    using ChatService.Interface;
    using ChatService.Models;
    using ChatService.Utilities;

        public class MessageService : IMessageService
    {
                private readonly ApiUtility _apiUtility;

                public MessageService(ApiUtility apiUtility)
        {
            _apiUtility = apiUtility;
        }

                public async Task<bool> StoreMessage(StoreUserMessage message)
        {

            var response = await _apiUtility.MessagePostApi<bool>("/api/usermessage/storemessage", message);

            return response.success;
        }
    }
}
