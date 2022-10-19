namespace IssueTracker.Utils;

public static class Constants
{
    public const string NotFound = " with id {0} does not exist in database";
    public const string Invalid = " field is invalid. ";
    public const string Null = " field cannot be null. ";
    public const string Placeholder = "placeholder";

    public const string Alphanumeric = @"^([\d\w-'_,.][\s]*)+\s*$";
    public static readonly List<string> AlertOptions = new() {"No alert", "Half hour", "1 hour", "1 day", "2 days"};
}