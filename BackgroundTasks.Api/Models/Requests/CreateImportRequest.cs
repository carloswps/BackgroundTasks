using System;

namespace BackgroundTasks.Api.Models.Requests;

public sealed class CreateImportRequest
{
    public int Limit { get; init; }
}
