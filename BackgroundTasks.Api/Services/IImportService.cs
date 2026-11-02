using System;
using BackgroundTasks.Api.Models.Requests;

namespace BackgroundTasks.Api.Services;

public interface IImportService
{
    Task<Guid> CreateImportAsync(CreateImportRequest request);
}
