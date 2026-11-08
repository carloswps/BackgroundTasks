using System;
using BackgroundTasks.Worker.Models;

namespace BackgroundTasks.Worker.Services;

public interface IDummyJsonService
{
    Task<DummyJsonProductsResponse> GetProductsResponseAsync(
        int limit,
        CancellationToken cancellationToken
    );
}
