namespace MessageManagementService.Models
{
    public class DBCachedMessages
    {
        public string messageContent { get; set; }
        public DateTime messageTime { get; set; }
        public int sendByUser { get; set; }
        public string messageId { get; set; }
    }
}
