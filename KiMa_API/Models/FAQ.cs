using System.ComponentModel.DataAnnotations;

namespace KiMa_API.Models
{
    public class FAQ
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Die Frage ist erforderlich.")]
        public string Question { get; set; }

        [Required(ErrorMessage = "Die Antwort ist erforderlich.")]
        public string Answer { get; set; }

        // Optional: Sortierreihenfolge, Kategorie, etc.
        public int Order { get; set; }
    }
}

