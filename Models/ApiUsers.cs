using System;
using System.ComponentModel.DataAnnotations;

namespace hcp.Models
{
    public class ApiUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string? FullName { get; set; }

        public string? Token { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;
    }
}
