using IssueTracker.Models;

namespace IssueTracker.Services;

public interface IReminderService
{
    Task<Reminder?> GetReminderById(int id);
    
    Task<Reminder> AddReminder(Reminder reminder);
    
    Task<Reminder?> UpdateReminder(Reminder reminder, int id);
    
    Task DeleteReminder(int id);
}