using KiMa_API.Models;

namespace KiMa_API.Models.Dto
{
    public class UserProfileUpdateDto
    {
        public VehicleCategory VehicleCategory { get; set; } = VehicleCategory.None;
        public string? VehicleDetails { get; set; }
        public string? Occupation { get; set; }
        public string? EducationLevel { get; set; }
        public string? Region { get; set; }
        public int? Age { get; set; }
        public string? IncomeLevel { get; set; }
        public bool IsInterestedInTechnology { get; set; }
        public bool IsInterestedInSports { get; set; }
        public bool IsInterestedInEntertainment { get; set; }
        public bool IsInterestedInTravel { get; set; }
    }
}

