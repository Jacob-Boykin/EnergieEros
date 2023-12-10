using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EnergieEros.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Role { get; set; } = "Anonymous"; // Admin/Customer/Anonymous
        public string ReversiblePassword { get; set; }
    }

}