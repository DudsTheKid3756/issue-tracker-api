namespace IssueTracker.Models;

public record Issue
{
    public int Id { get; set; }
    
    public string? Title { get; set; }
    
    public string? Comment { get; set; }
    
    public DateTime? Created { get; set; }
    
    public bool? IsCompleted { get; set; } = false;

    public bool? HasReminder { get; set; } = false;
}