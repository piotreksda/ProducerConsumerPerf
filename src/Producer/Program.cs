using EventCatalog;
using NServiceBus;
using Producer.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.UseOpenTelemetry();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging();

builder.RegisterProducer(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.MapGet("test", () => "Hello world publisher");

app.MapGet("test", async (IMessageSession messageSession) =>
{
    await messageSession.Publish(new TestEvent(Guid.NewGuid()));
});

app.Run();

