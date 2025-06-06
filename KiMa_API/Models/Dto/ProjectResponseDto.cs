using System.ComponentModel.DataAnnotations;

namespace KiMa_API.Models.Dto
{
    public class ProjectResponseDto
    {
        [Required]
        public int ProjectId { get; set; }
        public string? RespondentEmail { get; set; }
        [Required]
        public string AnswersJson { get; set; } = "";
        public DateTime SubmittedAt { get; set; }
    }
}
