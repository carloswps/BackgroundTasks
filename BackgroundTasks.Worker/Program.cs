using BackgroundTasks.Infrastructure.Configurations;
using BackgroundTasks.Worker;
using BackgroundTasks.Worker.Consumers;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<ImportProductsConsumer>();

IHost host = builder.Build();
host.Run();
