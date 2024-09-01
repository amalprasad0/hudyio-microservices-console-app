namespace CacheService.Interfaces
{
    public interface ISyncDataToSql
    {
        Task<bool> SyncDataToSql();
    }
}
