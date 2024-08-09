namespace CacheService.Interfaces
{
    public interface IRedisCache
    {
        object GetCacheData(string key);
        bool SetCacheData<T>(string key, T value, DateTimeOffset expirationTime);
        object RemoveData(string key);
    }
}
