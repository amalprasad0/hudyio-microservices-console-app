using CacheService.Helper;
using CacheService.Interfaces;
using StackExchange.Redis;
using Newtonsoft.Json;
namespace CacheService.Repository
{
    public class CacheRespository: IRedisCache
    {
        private IDatabase _db;
        public CacheRespository()
        {
            ConfigureRedis();
        }
        private void ConfigureRedis()
        {
            _db = ConnectionHelper.Connection.GetDatabase();
        }
        public object GetCacheData(string key)
        {
            var value = _db.StringGet(key);
            if (!string.IsNullOrEmpty(value))
            {
                return System.Text.Json.JsonSerializer.Deserialize<object>(value);
            }
            return null;
        }

        public object RemoveData(string key)
        {
            bool _isKeyExist = _db.KeyExists(key);
            if (_isKeyExist == true)
            {
                return _db.KeyDelete(key);
            }
            return false;
        }

        public bool SetCacheData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            var isSet = _db.StringSet(key, System.Text.Json.JsonSerializer.Serialize(value), expiryTime);
            return isSet;
        }

    }
}
