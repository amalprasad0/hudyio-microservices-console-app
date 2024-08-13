namespace CacheService.Models
{
    public class SaveRecentMessages
    {
        public string userId { get; set; }
        public List<string> Messages { get; set; }
    }
}
