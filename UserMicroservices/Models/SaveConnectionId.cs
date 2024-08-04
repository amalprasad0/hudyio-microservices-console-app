using System.ComponentModel.DataAnnotations;

namespace UserMicroservices.Models
{
    public class SaveConnectionId
    {
        [Required]
        public string ConnectionId { get; set; }
        [Required]
        public string MobileNumber { get; set; }
    }
}
