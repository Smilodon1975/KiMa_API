using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiMa_API.Models
{
    public enum VehicleCategory
    {
        None,
        Car,
        Motorcycle,
        Other
    }

    public class UserProfile
    {
        [Key, ForeignKey("User")]
        public int UserId { get; set; }

        public VehicleCategory VehicleCategory { get; set; } = VehicleCategory.None;

        public string? VehicleDetails { get; set; }

        [MaxLength(100)]
        public string? Occupation { get; set; }

        [MaxLength(50)]
        public string? EducationLevel { get; set; }

        [MaxLength(100)]
        public string? Region { get; set; }  

        public int? Age { get; set; }

        [MaxLength(50)]
        public string? IncomeLevel { get; set; }
        public bool IsInterestedInTechnology { get; set; }
        public bool IsInterestedInSports { get; set; }
        public bool IsInterestedInEntertainment { get; set; }
        public bool IsInterestedInTravel { get; set; }
    
        public virtual User? User { get; set; }

    }
}

