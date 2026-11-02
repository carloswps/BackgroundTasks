using BackgroundTasks.Api.Models.Requests;
using BackgroundTasks.Api.Services;
using BackgroundTasks.Contracts.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackgroundTasks.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ImportsController : ControllerBase
{
    private readonly IImportService _importService;

    public ImportsController(IImportService importService)
    {
        _importService = importService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateImport([FromBody] CreateImportRequest request)
    {
        var taskId = await _importService.CreateImportAsync(request);
        return Accepted(new { TaskId = taskId, Status = "Import requested." });
    }
}
