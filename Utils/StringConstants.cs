using IssueTracker.Models;

namespace IssueTracker.Utils;

public static class Constants
{
    public const string NotFound = " with id {0} does not exist in database";
    public const string Invalid = " field is invalid. ";
    public const string Null = " field cannot be null. ";
    public const string Placeholder = "placeholder";
    public const string TimeFormat = "HH:mm";
    public static readonly Reminder PlaceholderReminder = new()
    {
        Id = -1,
        Date = new DateOnly(1600, 01, 01),
        Time = new TimeOnly(0, 0),
        Alert = Placeholder,
        IssueId = -1
    };

    public const string Alphanumeric = @"^([\d\w-'_,.][\s]*)+\s*$";
    public static readonly List<string> AlertOptions = new() { "No alert", "Half hour", "1 hour", "1 day", "2 days" };
}