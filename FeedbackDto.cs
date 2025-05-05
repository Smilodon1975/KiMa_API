using KiMa_API.Models;

namespace KiMa_API.Models
{
    public class FeedbackDto
    {
        public int? UserId { get; set; }
        public string? Email { get; set; }
        public string Content { get; set; }
    }
}

