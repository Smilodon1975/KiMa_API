namespace KiMa_API.Models
{
    public class UserUpdateDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhonePrivate { get; set; }
        public string? PhoneMobile { get; set; }
        public string? PhoneWork { get; set; }
        public int? Age { get; set; }        
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }
        public string? Title { get; set; }
        public string? Status { get; set; }
        public string? Street { get; set; }
        public string? Zip { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }
}

