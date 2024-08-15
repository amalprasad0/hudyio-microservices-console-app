namespace CacheService.Models
{
    public class SaveRecentMessages
    {
        public string ToUserId { get; set; }
        public UserMessage MessageData { get; set; }
    }
}
