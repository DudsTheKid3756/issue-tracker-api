using Microsoft.EntityFrameworkCore;
using IssueTracker.Models;

namespace IssueTracker.Data;

public class IssueTrackerContext : DbContext
{
    public DbSet<Issue> Issues { get; set; } = null!;

    public IssueTrackerContext(DbContextOptions<IssueTrackerContext> options)
        : base(options) { }
}