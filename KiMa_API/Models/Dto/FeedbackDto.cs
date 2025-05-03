namespace KiMa_API.Models.Dto
{
    public class FeedbackDto
    {
        public int? UserId { get; set; }
        public string? Email { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
