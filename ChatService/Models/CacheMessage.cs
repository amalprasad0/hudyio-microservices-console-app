namespace ChatService.Models
{
    public class CacheMessage
    {
        public string ToUserId { get; set; }
        public UserMessage MessageData { get; set; }
    }
}
