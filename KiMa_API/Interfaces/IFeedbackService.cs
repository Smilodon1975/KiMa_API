using KiMa_API.Models;
using KiMa_API.Models.Dto;

public interface IFeedbackService
{
    Task SubmitAsync(FeedbackDto dto);
    Task<List<Feedback>> GetAllAsync();
    Task<bool> DeleteAsync(int id);
    
}

