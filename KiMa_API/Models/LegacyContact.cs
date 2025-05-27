

using CsvHelper.Configuration.Attributes;

namespace KiMa_API.Models
{
    public class LegacyContact
    {
        [Name("email")] public required string Email { get; set; }
        [Name("firstname")] public string? Firstname { get; set; }
        [Name("lastname")] public string? Lastname { get; set; }
        [Name("gender")] public string? Gender { get; set; }
        [Name("title")] public string? Title { get; set; }
        [Name("zip")] public string? Zip { get; set; }
        [Name("birthdate")] public DateTime? Birthdate { get; set; }
    }
}
