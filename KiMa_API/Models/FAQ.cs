using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Column("SortOrder")]
        public int Order { get; set; }
    }
}

