namespace ChatService.Models
{
    public class CacheMessage
    {
        public string ToUserId { get; set; }
        public List<UserMessage> MessageData { get; set; }
    }
}
