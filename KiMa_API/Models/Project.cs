﻿namespace KiMa_API.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? QuestionsJson { get; set; } = "[]";

        public ICollection<ProjectResponse>? Responses { get; set; }

        public ProjectStatus Status { get; set; } = ProjectStatus.Draft;
    }    
    
}
