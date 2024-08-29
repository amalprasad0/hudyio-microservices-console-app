using CacheService.Helper;
using CacheService.Interfaces;
using StackExchange.Redis;
using CacheService.Models;
using Newtonsoft.Json;
namespace CacheService.CacheMigration
{
    public class GetCachedData:IGetCachedData
    {
        private IDatabase _db;
        public GetCachedData(){
            _db = ConnectionHelper.Connection.GetDatabase();
        }
        public void GetCacheDataByUser()
        {
            //get the cache data by the user 
        }
        
    }
}
