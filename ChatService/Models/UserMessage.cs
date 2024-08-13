namespace ChatService.Models
{
    public class UserMessage
    {
        public int ToUserId { get; set; }
        public string MessageContent { get; set; }
        public DateTime MessageTime { get; set; }
        public string SendByUser { get; set; }
        
    }
}
