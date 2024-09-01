using CacheService.Helper;
using CacheService.Interfaces;
using CacheService.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace CacheService.Repository
{
    public class QueueRepository: IQueueCache
    {
        private IDatabase _db;
        public QueueRepository()
        {
            ConfigureRedis();
        }
        private void ConfigureRedis()
        {
            _db = ConnectionHelper.Connection.GetDatabase();
        }
        public Response<bool> EnqueueMessage(SaveRecentMessages messages)
        {
            var response = new Response<bool>();
            try
            {
                string serializedValue = System.Text.Json.JsonSerializer.Serialize(messages.MessageData);

                _db.ListRightPush(messages.ToUserId, serializedValue);
                _db.KeyExpire(messages.ToUserId, TimeSpan.FromHours(6));
                response.data = true;
                response.success =true;
            }
            catch(Exception ex) 
            {
                response.data = false;
                response.success=false;
                response.errorMessage=ex.Message;
            }
            return response;
        }
        public Response<List<T>> GetAllMessages<T>(string key)
        {
            var response = new Response<List<T>>();

            try
            {
                var serializedValues = _db.ListRange(key);
                if (serializedValues.Length > 0)
                {
                    response.data = serializedValues.Select(x => JsonConvert.DeserializeObject<T>(x)).ToList();
                    response.success = true;
                }
                else
                {
                    response.success = false;
                    response.errorMessage = "No messages found for the given key.";
                }
            }
            catch (Exception ex)
            {
                response.success = false;
                response.errorMessage = $"An error occurred while retrieving the messages: {ex.Message}";
            }

            return response;
        }
        public Response<bool> DequeueMessageById(string userId, string messageId)
        {
            var response = new Response<bool>();
            try
            {
                var messages = _db.ListRange(userId);

                foreach (var message in messages)
                {
                    var deserializedMessage = System.Text.Json.JsonSerializer.Deserialize<UserMessage>(message);

                    if (deserializedMessage.MessageId == messageId)
                    {
                        _db.ListRemove(userId, message); 
                        response.data = true;
                        response.success = true;
                        return response;
                    }
                }

                response.data = false; 
                response.success = false;
                response.errorMessage = "MessageId not found.";
            }
            catch (Exception ex)
            {
                response.data = false;
                response.success = false;
                response.errorMessage = ex.Message;
            }
            return response;
        }

    }
}
