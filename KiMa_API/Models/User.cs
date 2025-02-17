using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace KiMa_API.Models
{
    public class User : IdentityUser<int>  // IdentityUser verwaltet UserName & Email
    {
        public string Role { get; set; } = "Proband"; // Standardmäßig Proband
        public DateTime CreatedAt { get; set; }
        // Zusätzliche Probanden-Daten
        [Required, MaxLength(100)]
        public string FirstName { get; set; }

        [Required, MaxLength(100)]
        public string LastName { get; set; }

        public string? Phone { get; set; }
        public int Age { get; set; }
    }
}
