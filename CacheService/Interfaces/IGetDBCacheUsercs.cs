using CacheService.Models;

namespace CacheService.Interfaces
{
    public interface IGetDBCacheUsercs
    {
        Task<List<DBCachedUserIds>?> GetDBCachedMessageIds();
        Task<Response<bool>> RemoveDbCachedMSg();
    }
}
