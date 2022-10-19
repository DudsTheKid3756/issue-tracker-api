namespace IssueTracker.Models;

public record Reminder
{
    public int Id { get; set; }
    
    public DateOnly? Date { get; set; }
    
    public TimeOnly? Time { get; set; }
    
    public string? Alert { get; set; }
    
    public int IssueId { get; set; }
}