namespace MessageManagementService.Models
{
    public class CachedUserIds
    {
        public int userId { get; set; }
        public List<string> cacheIds { get; set; }
    }
}
