namespace KiMa_API.Models
{
    public class ProjectResponse
    {
        public int Id { get; set; }
        public int ProjectId { get; set; } 
        public string? RespondentEmail { get; set; } 
        public string? AnswersJson { get; set; }  
        public DateTime SubmittedAt { get; set; } 

      
        public Project? Project { get; set; }
    }
}
