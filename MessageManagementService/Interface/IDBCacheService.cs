using MessageManagementService.Models;
namespace MessageManagementService.Interface
{
    public interface IDBCacheService
    {
        Task<Response<List<CachedUserIds>>> GetDBCachedUserIds();
        Task<Response<List<DBCachedMessages>>> GetDBCachedMessages(int userId);
    }
}
