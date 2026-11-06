using BackgroundTasks.Worker.Consumers;

namespace BackgroundTasks.Worker;

public class Worker : BackgroundService
{
    private readonly ImportProductsConsumer _importProductsConsumer;
    private readonly ILogger<Worker> _logger;

    public Worker(ImportProductsConsumer importProductsConsumer, ILogger<Worker> logger)
    {
        _importProductsConsumer = importProductsConsumer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _importProductsConsumer.StartAsync();
        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
