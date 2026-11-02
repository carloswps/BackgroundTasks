using System;
using BackgroundTasks.Contracts.Messages;

namespace BackgroundTasks.Api.Services;

public interface ITaskPublisher
{
    Task PublishAsync(ImportProductMessage message);
}
