namespace ChatService.Models
{
    public class CacheRecord
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public DateTimeOffset Expiration { get; set; }
    }
}
