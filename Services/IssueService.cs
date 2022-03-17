using IssueTracker.Models;

namespace IssueTracker.Services;

public interface IIssueService
{
    Task<List<Issue>> GetIssues();
    Task<Issue?> GetIssueById(int id);
    Task<Issue> AddIssue(Issue issue);
    Task<Issue?> UpdateIssue(Issue issue, int id);
    Task DeleteIssue(int id);
}