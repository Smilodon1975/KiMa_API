using System.ComponentModel.DataAnnotations;

namespace KiMa_API.Models
{
    public class RegisterModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        public string? Phone { get; set; }
        public int Age { get; set; }
       
        public string? Address { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
