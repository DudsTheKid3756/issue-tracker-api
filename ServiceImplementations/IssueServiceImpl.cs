using IssueTracker.Data;
using IssueTracker.Exceptions;
using IssueTracker.Models;
using IssueTracker.Services;
using IssueTracker.Utils;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.ServiceImplementations;

public class IssueServiceImpl : IIssueService
{
    private readonly IssueTrackerContext _context;
    private readonly ILogger<IssueServiceImpl> _logger;

    public IssueServiceImpl(IssueTrackerContext context, ILogger<IssueServiceImpl> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Issue>> GetIssues()
    {
        var result = await _context.Issues.ToListAsync();
        _logger.LogInformation("Got all issues");
        return result;
    }

    public async Task<Issue?> GetIssueById(int id)
    {
        var result = await _context.Issues.FindAsync(id);
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
        issue.Created = DateTime.Now.ToUniversalTime();
        var errors = Validation.GetErrors(issue);
        if (errors.Length != 0)
        {
            _logger.LogError("{}", errors);
            throw new InvalidException(errors);
        }

        var result = await _context.Issues.AddAsync(issue);
        await _context.SaveChangesAsync();
        _logger.LogInformation("New issue added");
        return result.Entity;
    }

    public async Task<Issue?> UpdateIssue(Issue issue, int id)
    {
        var result = await _context.Issues.FindAsync(id);
        if (result is null)
        {
            _logger.LogError("{}{}", "Issue", Constants.NotFound.Replace("{0}", id.ToString()));
            throw new NotFoundException(string.Format($"Issue{Constants.NotFound}", id));
        }

        issue.Created = DateTime.Now.ToUniversalTime();
        var errors = Validation.GetErrors(issue);
        if (errors.Length != 0)
        {
            _logger.LogError("{}", errors);
            throw new InvalidException(errors);
        }

        result.Id = id;
        result.Title = issue.Title;
        result.Comment = issue.Comment;
        result.Created = issue.Created;
        result.IsCompleted = issue.IsCompleted;
        result.HasReminder = issue.HasReminder;

        await _context.SaveChangesAsync();
        _logger.LogInformation(
            "Issue with id {} updated at: {}. 'Created' field changed to updated time",
            id, result.Created
        );
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