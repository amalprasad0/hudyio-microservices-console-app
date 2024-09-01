﻿namespace ChatService.Models
{
    public class StoreUserMessage
    {
        public string cachedMessageId { get; set; }
        public string messageContent { get; set; }
        public int fromUserId { get; set; }
        public int toUserId { get; set; }
        public bool hasFile { get; set; }
        public string fileUrl { get; set; }
        public string fileType { get; set; }
    }
}
