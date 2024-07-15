using System.ComponentModel.DataAnnotations;

namespace UserMicroservices.Models
{
    public class LoginUser
    {
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; }
        public string Username { get; set; }= string.Empty;
    }
}
