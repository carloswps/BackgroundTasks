using System.Text.Json;
using BackgroundTasks.Contracts.Messages;
using BackgroundTasks.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BackgroundTasks.Worker.Consumers;

public class ImportProductsConsumer
{
    private readonly RabbitMqSettings _settings;

    private IConnection? _connection;
    private IChannel? _channel;

    public ImportProductsConsumer(IOptions<RabbitMqSettings> options)
    {
        _settings = options.Value;
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
            ImportProductMessage? message = JsonSerializer.Deserialize<ImportProductMessage>(
                envetArgs.Body.Span
            );

            if (message is null)
                return;

            Console.WriteLine($"TaskId: {message.TaskId}");
            Console.WriteLine($"Limit: {message.Limit}");

            await _channel.BasicAckAsync(envetArgs.DeliveryTag, multiple: false);
        };

        await _channel.BasicConsumeAsync(
            queue: "products.import",
            autoAck: false,
            consumer: consumer
        );
    }
}
