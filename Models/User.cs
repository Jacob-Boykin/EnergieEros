using System.ComponentModel.DataAnnotations;

namespace EnergieEros.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        public string? Username { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public string? Role { get; set; } // Admin/User
                                         // Add other user-related properties as needed (e.g., address, phone number, etc.)
    }
}
