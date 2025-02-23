using System.ComponentModel.DataAnnotations;

namespace KiMa_API.Models
{
    public class RegisterModel
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        public string? Gender { get; set; }
        public string? Title { get; set; }
        public string? Status { get; set; } = "active";

        // Telefonnummern
        public string? PhonePrivate { get; set; }
        public string? PhoneMobile { get; set; }
        public string? PhoneWork { get; set; }

        // Adresse
        public string? Street { get; set; }
        public string? Zip { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }
}
