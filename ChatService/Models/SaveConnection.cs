using System.ComponentModel.DataAnnotations;

namespace ChatService.Models
{
    public class SaveConnection
    {
        [Required]
        public string ConnectionId { get; set; }
        [Required]
        public string userId { get; set; }
    }
}
