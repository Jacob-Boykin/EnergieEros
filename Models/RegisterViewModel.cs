using System.ComponentModel.DataAnnotations;

namespace EnergieEros.Models
{
    // RegisterViewModel.cs
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }

        public bool IsAdmin { get; set; }
    }

}
