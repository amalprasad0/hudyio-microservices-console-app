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
        public Response<bool> EnqueueMessage<T>(string key,List<T> Messages)
        {
            var response = new Response<bool>();
            try
            {
                string serializedValue = System.Text.Json.JsonSerializer.Serialize(Messages);

                _db.ListRightPush(key, serializedValue);
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
        public Response<T> DequeueMessage<T>(string key)
        {
            var response = new Response<T>();

            try
            {
                var serializedValue = _db.ListLeftPop(key);

                if (serializedValue.HasValue)
                {
                    response.Data = JsonConvert.DeserializeObject<T>(serializedValue);
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    response.ErrorMessage = "No message found for the given key.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = $"An error occurred while dequeuing the message: {ex.Message}";
            }

            return response;
        }

    }
}
