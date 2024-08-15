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
                response.Data = true;
                response.Success=true;
               
            }
            catch(Exception ex) 
            {
                response.Data = false;
                response.Success=false;
                response.ErrorMessage=ex.Message;
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
                    response.Data = serializedValues.Select(x => JsonConvert.DeserializeObject<T>(x)).ToList();
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    response.ErrorMessage = "No messages found for the given key.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = $"An error occurred while retrieving the messages: {ex.Message}";
            }

            return response;
        }


    }
}
