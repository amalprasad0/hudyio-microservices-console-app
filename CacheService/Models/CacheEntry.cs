namespace CacheService.Models
{
    public class CacheEntry
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public DateTimeOffset Expiration { get; set; }
    }
}
