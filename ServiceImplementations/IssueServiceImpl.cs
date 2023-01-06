using IssueTracker.Data;
using IssueTracker.Exceptions;
using IssueTracker.Models;
using IssueTracker.Services;
using IssueTracker.Utils;
using IssueTracker.Utils.Formatters;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.CircuitBreaker;

namespace IssueTracker.ServiceImplementations;

public class IssueServiceImpl : IIssueService
{
    private readonly AsyncCircuitBreakerPolicy _circuitBreaker;
    private readonly IssueTrackerContext _context;
    private readonly ILogger<IssueServiceImpl> _logger;

    public IssueServiceImpl(IssueTrackerContext context, ILogger<IssueServiceImpl> logger)
    {
        _context = context;
        _logger = logger;

        _circuitBreaker = Policy
            .Handle<UnavailableException>()
            .AdvancedCircuitBreakerAsync(
                0.5,
                TimeSpan.FromSeconds(10),
                10,
                TimeSpan.FromSeconds(15)
            );
    }

    public async Task<List<Issue>> GetIssues()
    {
        var result = await _circuitBreaker.ExecuteAsync(() => _context.Issues.Include(i => i.Reminder).ToListAsync());
        _logger.LogInformation("Got all issues");
        return result;
    }

    public async Task<Issue?> GetIssueById(int id)
    {
        var result = await _context.Issues.Include(i => i.Reminder).FirstOrDefaultAsync(i => i.Id == id);
        if (result is null)
        {
            _logger.LogError("{}{}", "Issue", Constants.NotFound.Replace("{0}", id.ToString()));
            throw new NotFoundException(string.Format($"Issue{Constants.NotFound}", id));
        }

        _logger.LogInformation("Got issue with id: {}", id);
        return result;
    }

    public async Task<Issue> AddIssue(Issue issue)
    {
        var reminder = issue.Reminder;
        issue.Created = Constants.Placeholder;
        var errors = "";
        var message = "";
        
        if (issue.HasReminder is true)
        {
            if (reminder is null)
            {
                errors += $"Reminder{Constants.Null}";
                _logger.LogError("{}", errors);
                throw new InvalidException(errors);
            }
            var options = Constants.AlertOptions;
            var formattedOptions = ListFormatter.Formatter(options);
            if (!options.Contains(reminder.Alert!)) errors += $"Alert{Constants.Invalid}Try '{formattedOptions}'. ";
        }
        else
        {
            reminder = null;
            message =
                "Issue was added without set Reminder as 'hasReminder' was left 'false'. Update Issue to add Reminder";
        }

        issue.Reminder = Constants.PlaceholderReminder;
        issue.Color = issue.Color![1..];

        errors += Validation.GetErrors(issue);

        issue.Color = $"#{issue.Color!}";
        errors += Validation.CheckHexCode(issue.Color!);

        if (errors.Length != 0)
        {
            _logger.LogError("{}", errors);
            throw new InvalidException(errors);
        }

        issue.Reminder = reminder;
        issue.Created = DateTime.Now.ToLocalTime().ToString("G");
        var result = await _context.Issues.AddAsync(issue);
        await _context.SaveChangesAsync();
        _logger.LogInformation("{}", message.Equals("") ? "New issue added" : message);
        return result.Entity;
    }

    public async Task<Issue?> UpdateIssue(Issue issue, int id)
    {
        var reminder = issue.Reminder;
        var result = await _context.Issues.Include(i => i.Reminder).FirstOrDefaultAsync(i => i.Id == id);
        if (result is null)
        {
            _logger.LogError("{}{}", "Issue", Constants.NotFound.Replace("{0}", id.ToString()));
            throw new NotFoundException(string.Format($"Issue{Constants.NotFound}", id));
        }

        issue.Created = Constants.Placeholder;
        var errors = "";
        var message = "";
        if (issue.HasReminder is true)
        {
            if (reminder is null)
            {
                errors += $"Reminder{Constants.Null}";
                _logger.LogError("{}", errors);
                throw new InvalidException(errors);
            }
            var options = Constants.AlertOptions;
            var formattedOptions = ListFormatter.Formatter(options);
            if (!options.Contains(reminder.Alert!)) errors += $"Alert{Constants.Invalid}Try '{formattedOptions}'";
        }
        else message = "'hasReminder' field was false. Reminder was set to null";

        issue.Reminder = Constants.PlaceholderReminder;
        issue.Color = issue.Color![1..];

        errors += Validation.GetErrors(issue);

        issue.Color = $"#{issue.Color!}";
        errors += Validation.CheckHexCode(issue.Color);

        if (errors.Length != 0)
        {
            _logger.LogError("{}", errors);
            throw new InvalidException(errors);
        }

        result.Id = id;
        result.Title = issue.Title;
        result.Comment = issue.Comment;
        result.Color = issue.Color;
        result.Created = result.Created;
        result.IsCompleted = issue.IsCompleted;
        result.HasReminder = issue.HasReminder;

        result.Reminder = result.HasReminder is true ? reminder : null;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Issue with id {} updated. {}", id, message);
        return result;
    }

    public async Task DeleteIssue(int id)
    {
        var result = await _context.Issues.FindAsync(id);
        if (result is null)
        {
            _logger.LogError("{}{}", "Issue", Constants.NotFound.Replace("{0}", id.ToString()));
            throw new NotFoundException(string.Format($"Issue{Constants.NotFound}", id));
        }

        _context.Issues.Remove(result);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Issue with id {} deleted", id);
    }
}