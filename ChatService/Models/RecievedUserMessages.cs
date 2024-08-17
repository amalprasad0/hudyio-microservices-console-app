namespace ChatService.Models
{
    public class RecievedUserMessages
    {

        public string messageContent { get; set; }
        public DateTime messageTime { get; set; }
        public string sendByUser { get; set; }
        public string messageId { get; set; }
    }
}
