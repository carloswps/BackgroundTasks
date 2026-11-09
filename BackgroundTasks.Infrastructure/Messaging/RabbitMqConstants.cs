using System;

namespace BackgroundTasks.Infrastructure.Messaging;

public static class RabbitMqConstants
{
    public const string ImportQueue = "products.import";

    public const string RetryQueue = "products.import.retry";

    public const string DeadLetterQueue = "products.import.dlq";

    public const string DeadLetterExchange = "products.import.dlx";
}
