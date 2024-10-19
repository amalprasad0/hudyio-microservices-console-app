using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Google.Protobuf.WellKnownTypes;
using Messagemanagementservice;

namespace ChatService.GprcClient
{
    public class MessageServiceClient
    {
        private readonly MessageService.MessageServiceClient _client;

        public MessageServiceClient(GrpcChannel channel)
        {
            _client = new MessageService.MessageServiceClient(channel);
        }

        public async Task<ApiResponse> StoreUserMessageAsync(userMessage message)
        {
            var response = await _client.StoreUserMessageAsync(message);
            return response;
        }
        public async Task<ApiResponse> StoreCachedMessageAsync(storeCachedMessageRequest request)
        {
            var response = await _client.StoreCachedMessageAsync(request);
            return response;
        }
        public async Task<CachedUserIdsResponse> GetDBCachedUserIdsAsync()
        {
            var response = await _client.GetDBCachedUserIdsAsync(new Empty());
            return response;
        }
        public async Task<DBCachedMessagesResponse> GetCachedMessagesAsync(int userId)
        {
            var request = new GetCachedMessagesRequest { UserId = userId };
            var response = await _client.GetCachedMessagesAsync(request);
            return response;
        }
    }
}
