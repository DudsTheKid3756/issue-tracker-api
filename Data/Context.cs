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
            .HasOne(i => i.Reminder)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}

// dotnet-ef database drop --context IssueTrackerContext && dotnet-ef migrations remove --context AuthContext && dotnet-ef migrations remove --context IssueTrackerContext && dotnet-ef migrations add InitialCreate --context IssueTrackerContext && dotnet-ef migrations add InitialCreate --context AuthContext && dotnet-ef database update --context IssueTrackerContext && dotnet-ef database update --context AuthContext
