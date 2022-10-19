using Microsoft.EntityFrameworkCore;
using IssueTracker.Models;

namespace IssueTracker.Data;

public class IssueTrackerContext : DbContext
{
    public DbSet<Issue> Issues { get; set; } = null!;
    public DbSet<Reminder> Reminders { get; set; } = null!;

    public IssueTrackerContext(DbContextOptions<IssueTrackerContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Issue>()
            .HasOne(i => i.Reminder);
    }
}