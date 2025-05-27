using System.ComponentModel.DataAnnotations;


namespace KiMa_API.Models
{
    public class News
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Der Titel ist erforderlich.")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Der Inhalt ist erforderlich.")]
        public string? Content { get; set; }

        // Datum der Veröffentlichung – standardmäßig auf jetzt gesetzt
        public DateTime PublishDate { get; set; } = DateTime.Now;
    }
}

