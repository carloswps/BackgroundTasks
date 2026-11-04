using System;
using BackgroundTasks.Api.Models.Requests;
using BackgroundTasks.Contracts.Messages;
using BackgroundTasks.Infrastructure.Services;

namespace BackgroundTasks.Api.Services;

public class ImportService : IImportService
{
    private readonly ITaskPublisher _taskPublisher;

    public ImportService(ITaskPublisher taskPublisher)
    {
        _taskPublisher = taskPublisher;
    }

    async Task<Guid> IImportService.CreateImportAsync(CreateImportRequest request)
    {
        var message = new ImportProductMessage { TaskId = Guid.NewGuid(), Limit = request.Limit };
        await _taskPublisher.PublishAsync(message);
        return message.TaskId;
    }
}
