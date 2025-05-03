﻿namespace KiMa_API.Models
{

    public class NewsletterSubscriber
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public bool IsSubscribed { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }
}