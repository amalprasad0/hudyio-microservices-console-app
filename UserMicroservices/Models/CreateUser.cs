using System.ComponentModel.DataAnnotations;

namespace UserMicroservices.Models
{
    public class CreateUser
    {
        [Required]
        public string Name { get; set; }
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string IsDisabled { get; set; }
       
    }
}
