using System.Text.Json;
using BackgroundTasks.Contracts.Messages;
using BackgroundTasks.Infrastructure.Configurations;
using BackgroundTasks.Worker.Services;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BackgroundTasks.Worker.Consumers;

public class ImportProductsConsumer
{
    private readonly RabbitMqSettings _settings;

    private IConnection? _connection;
    private IChannel? _channel;

    private readonly IDummyJsonService _dummyJsonService;

    public ImportProductsConsumer(
        IOptions<RabbitMqSettings> options,
        IDummyJsonService dummyJsonService
    )
    {
        _settings = options.Value;
        _dummyJsonService = dummyJsonService;
    }

    public async Task StartAsync()
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password,
        };

        _connection ??= await factory.CreateConnectionAsync();
        _channel ??= await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(
            queue: "products.import",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, envetArgs) =>
        {
            try
            {
                ImportProductMessage? message = JsonSerializer.Deserialize<ImportProductMessage>(
                    envetArgs.Body.Span
                );

                if (message is null)
                    throw new Exception("Message is null or invalid.");

                Console.WriteLine($"TaskId: {message.TaskId}");
                Console.WriteLine($"Limit: {message.Limit}");

                var result = await _dummyJsonService.GetProductsResponseAsync(
                    message.Limit,
                    CancellationToken.None
                );

                Console.WriteLine($"Total Products: {result.Products.Count}");

                foreach (var product in result.Products)
                {
                    Console.WriteLine($"Product: {product.Title}, Price: {product.Price}");
                }

                await _channel.BasicAckAsync(envetArgs.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
                await _channel.BasicNackAsync(
                    envetArgs.DeliveryTag,
                    multiple: false,
                    requeue: true
                );
            }
        };

        await _channel.BasicConsumeAsync(
            queue: "products.import",
            autoAck: false,
            consumer: consumer
        );
    }
}
