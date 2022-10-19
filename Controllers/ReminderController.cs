using IssueTracker.Models;
using IssueTracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Controllers;

[ApiController]
[Route("[controller]")]
public class ReminderController : ControllerBase
{
    private readonly IReminderService _reminderService;
    private readonly ILogger<ReminderController> _logger;

    public ReminderController(IReminderService reminderService, ILogger<ReminderController> logger)
    {
        _reminderService = reminderService;
        _logger = logger;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Reminder?>> GetReminderById(int id)
    {
        _logger.LogInformation("Request received for GetReminderById: {}", id);
        return await _reminderService.GetReminderById(id);
    }

    [HttpPost]
    public async Task<IActionResult> AddReminder(Reminder reminder)
    {
        _logger.LogInformation("Request received for AddReminder");
        var result = await _reminderService.AddReminder(reminder);
        return Created("", result);
    }
    
    [HttpPut("{id:int}")]
    public async Task<ActionResult<Reminder?>> UpdateReminder(Reminder reminder, int id)
    {
        _logger.LogInformation("Request received for UpdateReminder: {}", id);
        return await _reminderService.UpdateReminder(reminder, id);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteReminder(int id)
    {
        _logger.LogInformation("Request received for DeleteReminder: {}", id);
        await _reminderService.DeleteReminder(id);
        return NoContent();
    }
}