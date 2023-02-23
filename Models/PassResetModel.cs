using System.ComponentModel.DataAnnotations;

namespace IssueTracker.Models;

public record PassResetModel
{
    public string? Username { get; set; }

    public string? CurrPassword { get; set; }

    public string? NewPassword { get; set; }

    public string? RepeatPassword { get; set; }
}