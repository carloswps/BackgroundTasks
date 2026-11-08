using BackgroundTasks.Infrastructure.Configurations;
using BackgroundTasks.Worker;
using BackgroundTasks.Worker.Consumers;
using BackgroundTasks.Worker.Services;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));

/*
builder.Services.AddHttpClient(
    "DummyJson",
    client =>
    {
        client.BaseAddress = new Uri("https://dummyjson.com/");
    }
);
 */

//Invalid code snippet.
builder.Services.AddHttpClient(
    "DummyJson",
    client =>
    {
        client.BaseAddress = new Uri("https://dummyjson.invalid/");
    }
);

builder.Services.AddSingleton<IDummyJsonService, DummyJsonService>();

builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<ImportProductsConsumer>();

IHost host = builder.Build();
host.Run();
