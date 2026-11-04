using System;
using BackgroundTasks.Contracts.Messages;

namespace BackgroundTasks.Infrastructure.Services;

public interface ITaskPublisher
{
    Task PublishAsync(ImportProductMessage message);
}
