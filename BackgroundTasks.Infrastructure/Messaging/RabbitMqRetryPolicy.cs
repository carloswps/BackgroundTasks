using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace BackgroundTasks.Infrastructure.Messaging;

public class RabbitMqRetryPolicy
{
    public static int GetRejectedCount(IDictionary<string, object?>? headers)
    {
        if (
            headers is null
            || !headers.TryGetValue("x-death", out var xDeathValue)
            || xDeathValue is not IList deaths
        )
        {
            return 0;
        }

        foreach (var item in deaths)
        {
            if (item is not IDictionary deathEntry)
                continue;

            string queue = ReadString(deathEntry["queue"]);
            string reason = ReadString(deathEntry["reason"]);

            if (queue == RabbitMqConstants.ImportQueue && reason == "rejected")
            {
                return Convert.ToInt32(deathEntry["count"], CultureInfo.InvariantCulture);
            }
        }

        return 0;
    }

    private static string ReadString(object? value)
    {
        return value switch
        {
            string text => text,
            byte[] bytes => Encoding.UTF8.GetString(bytes),
            _ => string.Empty,
        };
    }
}
