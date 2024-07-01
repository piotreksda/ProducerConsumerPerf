using Consumer.Configuration;
using NServiceBus;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace Consumer.Extensions;

public static class ConfigureBuilder
{
    public static WebApplicationBuilder RegisterSubscriber(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        RabbitMqOptions rabbitMqOptions = new();
        configuration.GetSection(RabbitMqOptions.Section).Bind(rabbitMqOptions);

        rabbitMqOptions.Validate();
        
        var connectionString = $"amqp://{rabbitMqOptions.User}:{rabbitMqOptions.Password}@{rabbitMqOptions.Host}";
        
        var endpointConfiguration = new EndpointConfiguration("Consumer");
        var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
        transport.ConnectionString(connectionString);
        transport.UseConventionalRoutingTopology(QueueType.Quorum);
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
        endpointConfiguration.EnableInstallers();
        
        endpointConfiguration.EnableOpenTelemetry();

        builder.UseNServiceBus(endpointConfiguration);

        return builder;
    }
    
    public static WebApplicationBuilder UseOpenTelemetry(this WebApplicationBuilder builder)
    {
        var serviceName = "Consumer";
        var serviceVersion = "1.0.0";

        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName, serviceVersion: serviceVersion);

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
                {
                    metrics.SetResourceBuilder(resourceBuilder);
                    metrics.AddMeter("Microsoft.AspNetCore.Hosting");
                    metrics.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
                    metrics.AddMeter("System.Net.Http");
                    metrics.AddMeter("NServiceBus.Core");
                    metrics.AddOtlpExporter((exporterOptions, metricReaderOptions) =>
                    {
                        metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 5000;
                    });
                }
            )
            .WithTracing(traces =>
            {
                traces.AddSource("NServiceBus.Core");
            });
        
        return builder;
    }
    
    
}