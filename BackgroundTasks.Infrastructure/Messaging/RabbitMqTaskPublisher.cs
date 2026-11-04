using System.Text;
using System.Text.Json;
using BackgroundTasks.Contracts.Messages;
using BackgroundTasks.Infrastructure.Configurations;
using BackgroundTasks.Infrastructure.Services;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace BackgroundTasks.Infrastructure.Messaging;

public class RabbitMqTaskPublisher : ITaskPublisher
{
    private readonly RabbitMqSettings _settings;

    public RabbitMqTaskPublisher(IOptions<RabbitMqSettings> options)
    {
        _settings = options.Value;
    }

    public async Task PublishAsync(ImportProductMessage message)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password,
        };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            "products.import",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = new BasicProperties { Persistent = true };

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: "products.import",
            mandatory: true,
            basicProperties: properties,
            body: body
        );
    }
}
