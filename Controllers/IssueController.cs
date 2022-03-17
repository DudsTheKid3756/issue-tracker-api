using IssueTracker.Models;
using IssueTracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Controllers;

[ApiController]
[Route("[controller]")]
public class IssueController : ControllerBase
{
    private readonly IIssueService _issueService;
    private readonly ILogger<IssueController> _logger;
    
    public IssueController(IIssueService issueService, ILogger<IssueController> logger)
    {
        _issueService = issueService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<Issue>>> GetIssues()
    {
        _logger.LogInformation("Request received for GetIssues");
        return await _issueService.GetIssues();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Issue?>> GetIssueById(int id)
    {
        _logger.LogInformation("Request received for GetIssueById: {}", id);
        return await _issueService.GetIssueById(id);
    }

    [HttpPost]
    public async Task<IActionResult> AddIssue(Issue issue)
    {
        _logger.LogInformation("Request received for AddIssue");
        var result = await _issueService.AddIssue(issue);
        return Created("", result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Issue?>> UpdateIssue(Issue issue, int id)
    {
        _logger.LogInformation("Request received for UpdateIssue: {}", id);
        return await _issueService.UpdateIssue(issue, id);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteIssue(int id)
    {
        _logger.LogInformation("Request received for DeleteIssue: {}", id);
        await _issueService.DeleteIssue(id);
        return NoContent();
    }
}