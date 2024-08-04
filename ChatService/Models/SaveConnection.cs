using System.ComponentModel.DataAnnotations;

namespace ChatService.Models
{
    public class SaveConnection
    {
        [Required]
        public string ConnectionId { get; set; }
        [Required]
        public string MobileNumber { get; set; }
    }
}
