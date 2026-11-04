namespace BackgroundTasks.Api.Configurations;

public class RabbitMqSettings
{
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; }
    public string UserName { get; set; } = "app";
    public string Password { get; set; } = string.Empty;
}
