using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace KiMa_API.Models
{
    public class User : IdentityUser<int>
    {
        /// Erweiterte Benutzerklasse, die von `IdentityUser<int>` erbt und zusätzliche Eigenschaften enthält.
        public string Role { get; set; } = "Proband";

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }


        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)] // Falls es Zeichenlimit gibt
        public override string UserName { get; set; } = string.Empty;

        public string? Title { get; set; } = null;
        public string? Gender { get; set; } = null;
        public string? Status { get; set; } = "active";

        // ✅ Mehrere Telefonnummern
        public string? PhonePrivate { get; set; } = null;
        public string? PhoneMobile { get; set; } = null;
        public string? PhoneWork { get; set; } = null;

        public int Age { get; set; } = 0;
        public DateTime? BirthDate { get; set; } = null;

        // ✅ Adresse aufgeteilt
        public string? Street { get; set; } = null;
        public string? Zip { get; set; } = null;
        public string? City { get; set; } = null;
        public string? Country { get; set; } = null;

        // ✅ Data Consent
        public bool NewsletterSub { get; set; } = false;

        public virtual UserProfile? UserProfile { get; set; }


    }
}

