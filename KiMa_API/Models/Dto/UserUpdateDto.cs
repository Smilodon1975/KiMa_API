using System.ComponentModel.DataAnnotations;

namespace KiMa_API.Models.Dto
{
    public class UserUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [StringLength(50, ErrorMessage = "Der Benutzername darf maximal 50 Zeichen lang sein.")]
        public string? UserName { get; set; }

        [EmailAddress(ErrorMessage = "Ungültige E-Mail-Adresse.")]
        public string Email { get; set; }

        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        public string? Title { get; set; }
        public string? Gender { get; set; }
        public string? Status { get; set; } = "active";

        // ✅ Telefonnummern
        public string? PhonePrivate { get; set; }
        public string? PhoneMobile { get; set; }
        public string? PhoneWork { get; set; }

        public int? Age { get; set; }
        public DateTime? BirthDate { get; set; }

        // ✅ Adresse
        public string? Street { get; set; }
        public string? Zip { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Role { get; set; }
        public string? Password { get; set; }
        public bool? DataConsent { get; set; }
        public UserProfileUpdateDto? Profile { get; set; }

    }
}
