namespace CacheService.Models
{
    public class SaveRecentMessages
    {
        public string ToUserId { get; set; }
        public List<UserMessage> MessageData { get; set; }
    }
}
