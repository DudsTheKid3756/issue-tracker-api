using System.Text.Json.Serialization;
using IssueTracker.Utils.JsonConverters;

namespace IssueTracker.Models;

public record Reminder
{
    public int? Id { get; set; }

    [property: JsonConverter(typeof(DateOnlyConverter))]
    public DateOnly? Date { get; set; }

    [property: JsonConverter(typeof(TimeOnlyConverter))]
    public TimeOnly? Time { get; set; }

    public string? Alert { get; set; }

    public int? IssueId { get; set; }
}