using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class BasicUser
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }
    }
}
