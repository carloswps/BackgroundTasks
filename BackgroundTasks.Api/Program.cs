using BackgroundTasks.Api.Services;
using BackgroundTasks.Infrastructure.Configurations;
using BackgroundTasks.Infrastructure.Messaging;
using BackgroundTasks.Infrastructure.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddScoped<IImportService, ImportService>();
builder.Services.AddScoped<ITaskPublisher, RabbitMqTaskPublisher>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRouting();
app.MapControllers();

app.Run();
