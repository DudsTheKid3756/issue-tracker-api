using IssueTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Data;

public class IssueTrackerContext : DbContext
{
    public IssueTrackerContext(DbContextOptions<IssueTrackerContext> options)
        : base(options)
    {
    }

    public DbSet<Issue> Issues { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Issue>()
            .HasOne(i => i.Reminder).WithOne();
    }
}