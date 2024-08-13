namespace ChatService.Models
{
    public class CacheMessage
    {
        public string userId { get; set; }
        List<UserMessage> messages { get; set; }
    }
}
