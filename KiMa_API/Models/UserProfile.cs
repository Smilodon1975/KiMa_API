using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiMa_API.Models
{
    // Enum für Fahrzeugkategorie
    public enum VehicleCategory
    {
        None,
        Car,
        Motorcycle,
        Other
    }

    public class UserProfile
    {
        // Primärschlüssel, der gleichzeitig der Fremdschlüssel zum User ist
        [Key, ForeignKey("User")]
        public int UserId { get; set; }

        // Fahrzeugbezogene Angaben
        public VehicleCategory VehicleCategory { get; set; } = VehicleCategory.None;

        // Details zum Fahrzeug (z. B. "Audi TT" oder "Honda CBR"), falls vorhanden
        public string? VehicleDetails { get; set; }

        // Weitere Angaben
        [MaxLength(100)]
        public string? Occupation { get; set; }

        [MaxLength(50)]
        public string? EducationLevel { get; set; }

        [MaxLength(100)]
        public string? Region { get; set; }  // z. B. Wohnort oder Region

        public int? Age { get; set; }

        [MaxLength(50)]
        public string? IncomeLevel { get; set; }  // z. B. "niedrig", "mittel", "hoch"

        // Zusätzliche Interessen (je nach Projektanforderungen)
        public bool IsInterestedInTechnology { get; set; }
        public bool IsInterestedInSports { get; set; }
        public bool IsInterestedInEntertainment { get; set; }
        public bool IsInterestedInTravel { get; set; }

        // Navigationseigenschaft zum User
        public virtual User User { get; set; }

    }
}

