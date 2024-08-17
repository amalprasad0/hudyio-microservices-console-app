namespace ChatService.Models
{
    public class UserMessage
    {
        public string MessageContent { get; set; }
        public DateTime MessageTime { get; set; }
        public string SendByUser { get; set; }

        public string MessageId { get; set; }
        
    }
}
