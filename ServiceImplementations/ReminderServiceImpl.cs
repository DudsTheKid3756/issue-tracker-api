using IssueTracker.Data;
using IssueTracker.Exceptions;
using IssueTracker.Models;
using IssueTracker.Services;
using IssueTracker.Utils;

namespace IssueTracker.ServiceImplementations;

public class ReminderServiceImpl : IReminderService
{
    private readonly IssueTrackerContext _context;
    private readonly ILogger<ReminderServiceImpl> _logger;

    public ReminderServiceImpl(IssueTrackerContext context, ILogger<ReminderServiceImpl> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Reminder?> GetReminderById(int id)
    {
        var result = await _context.Reminders.FindAsync(id);
        if (result is null)
        {
            _logger.LogError("{}{}", "Reminder", Constants.NotFound.Replace("{0}", id.ToString()));
            throw new NotFoundException(string.Format($"Reminder{Constants.NotFound}", id));
        }

        _logger.LogInformation("Got reminder with id: {}", id);
        return result;
    }

    public async Task<Reminder> AddReminder(Reminder reminder)
    {
        if (Constants.AlertOptions.Contains(reminder.Alert!))
        {
            _logger.LogError("Alert field invalid. Try {}", Constants.AlertOptions);
        }
    }
}