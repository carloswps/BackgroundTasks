using BackgroundTasks.Api.Models.Requests;
using BackgroundTasks.Api.Services;
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
        Guid taskId = await _importService.CreateImportAsync(request);
        return Accepted(new { TaskId = taskId, Status = "Import requested." });
    }
}
