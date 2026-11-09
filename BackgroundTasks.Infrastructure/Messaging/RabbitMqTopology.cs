using System;
using System.Formats.Asn1;
using RabbitMQ.Client;

namespace BackgroundTasks.Infrastructure.Messaging;

public static class RabbitMqTopology
{
    public static async Task ConfigureAsync(IChannel channel)
    {
        await channel.ExchangeDeclareAsync(
            exchange: RabbitMqConstants.DeadLetterExchange,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            arguments: null
        );

        var mainQueueArguments = new Dictionary<string, object?>
        {
            { "x-dead-letter-exchange", RabbitMqConstants.DeadLetterExchange },
            { "x-dead-letter-routing-key", RabbitMqConstants.RetryQueue },
        };

        await channel.QueueDeclareAsync(
            queue: RabbitMqConstants.ImportQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: mainQueueArguments
        );

        var retryQueueArguments = new Dictionary<string, object?>
        {
            ["x-message-ttl"] = 10_000, // 10 seconds
            ["x-dead-letter-exchange"] = RabbitMqConstants.DeadLetterExchange,
            ["x-dead-letter-routing-key"] = RabbitMqConstants.ImportQueue,
        };

        await channel.QueueDeclareAsync(
            queue: RabbitMqConstants.RetryQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: retryQueueArguments
        );

        await channel.QueueBindAsync(
            queue: RabbitMqConstants.RetryQueue,
            exchange: RabbitMqConstants.DeadLetterExchange,
            routingKey: RabbitMqConstants.RetryQueue
        );

        await channel.QueueDeclareAsync(
            queue: RabbitMqConstants.DeadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
    }
}
