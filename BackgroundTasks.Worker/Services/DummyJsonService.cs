using System;
using System.Net.Http.Json;
using BackgroundTasks.Worker.Models;

namespace BackgroundTasks.Worker.Services;

public class DummyJsonService : IDummyJsonService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public DummyJsonService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<DummyJsonProductsResponse> GetProductsResponseAsync(
        int limit,
        CancellationToken cancellationToken
    )
    {
        var httpClient = _httpClientFactory.CreateClient("DummyJson");

        var response = await httpClient.GetFromJsonAsync<DummyJsonProductsResponse>(
            $"products?limit={limit}",
            cancellationToken
        );

        return response ?? new DummyJsonProductsResponse();
    }
}
