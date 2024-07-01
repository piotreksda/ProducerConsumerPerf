using Consumer.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.UseOpenTelemetry();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging();

builder.RegisterSubscriber(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("test", () => "Hello world consumer");

app.Run();