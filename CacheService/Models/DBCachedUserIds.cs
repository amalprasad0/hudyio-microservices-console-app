namespace CacheService.Models
{
    public class DBCachedUserIds
    {
        public int userId { get; set; }
        public List<string> cacheIds { get; set; }
    }
}
