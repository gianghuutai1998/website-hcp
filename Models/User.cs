using System.ComponentModel.DataAnnotations;

namespace hcp.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } = "User"; // "Admin" hoặc "User"
    }
}
