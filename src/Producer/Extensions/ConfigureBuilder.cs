using EventCatalog;
using NServiceBus;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Subscriber.Configuration;

namespace Producer.Extensions;

public static class ConfigureBuilder
{
    public static WebApplicationBuilder RegisterProducer(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        RabbitMqOptions rabbitMqOptions = new();
        configuration.GetSection(RabbitMqOptions.Section).Bind(rabbitMqOptions);

        rabbitMqOptions.Validate();
        
        var connectionString = $"amqp://{rabbitMqOptions.User}:{rabbitMqOptions.Password}@{rabbitMqOptions.Host}";
        
        var endpointConfiguration = new EndpointConfiguration("Producer");
        var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
        transport.ConnectionString(connectionString);
        transport.UseConventionalRoutingTopology(QueueType.Quorum);
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
        endpointConfiguration.EnableInstallers();
        
        endpointConfiguration.SendOnly();
        
        endpointConfiguration.EnableOpenTelemetry();
        
        // endpointConfiguration.EnableMetrics();

        // var routing = transport.Routing();
        // routing.RouteToEndpoint(
        //     assembly: typeof(TestEvent).Assembly,
        //     destination: "Consumer");

        builder.UseNServiceBus(endpointConfiguration);
        
        return builder;
    }

    public static WebApplicationBuilder UseOpenTelemetry(this WebApplicationBuilder builder)
    {
        var serviceName = "Producer";
        var serviceVersion = "1.0.0"; // You can also specify the version of your service

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