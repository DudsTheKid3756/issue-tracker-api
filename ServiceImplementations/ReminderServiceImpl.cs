using IssueTracker.Data;
using IssueTracker.Exceptions;
using IssueTracker.Models;
using IssueTracker.Services;
using IssueTracker.Utils;
using Microsoft.AspNetCore.Mvc;

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
        var options = Constants.AlertOptions;
        if (!options.Contains(reminder.Alert!))
        {
            var error = $"Alert field invalid. Try {ListFormatter.Formatter(options)}";
            _logger.LogError("{}", error);
            throw new InvalidException(error);
        }

        var result = await _context.Reminders.AddAsync(reminder);
        await _context.SaveChangesAsync();
        _logger.LogInformation("New reminder added");
        return result.Entity;
    }

    public async Task<Reminder?> UpdateReminder(Reminder reminder, int id)
    {
        var result = await _context.Reminders.FindAsync(id);
        if (result is null)
        {
            _logger.LogError("{}{}", "Reminder", Constants.NotFound.Replace("{0}", id.ToString()));
            throw new NotFoundException(string.Format($"Reminder{Constants.NotFound}", id));
        }
        
        var options = Constants.AlertOptions;
        if (!options.Contains(reminder.Alert!))
        {
            var error = $"Alert field invalid. Try {ListFormatter.Formatter(options)}";
            _logger.LogError("{}", error);
            throw new InvalidException(error);
        }

        result.Id = id;
        result.Date = reminder.Date;
        result.Time = reminder.Time;
        result.Alert = reminder.Alert;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Reminder with id {} updated", id);
        return result;
    }

    public async Task DeleteReminder(int id)
    {
        var result = await _context.Reminders.FindAsync(id);
        if (result is null)
        {
            _logger.LogError("{}{}", "Reminder", Constants.NotFound.Replace("{0}", id.ToString()));
            throw new NotFoundException(string.Format($"Reminder{Constants.NotFound}", id));
        }
        
        _context.Reminders.Remove(result);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Reminder with id {} deleted", id);
    }
}