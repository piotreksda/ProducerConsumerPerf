using EventCatalog;
using NServiceBus;

namespace Consumer.Consumers;

public record TestConsumer : IHandleMessages<TestEvent>
{
    private readonly ILogger<TestConsumer> _logger;

    public TestConsumer(ILogger<TestConsumer> logger)
    {
        _logger = logger;
    }

    public Task Handle(TestEvent message, IMessageHandlerContext context)
    {
        _logger.LogInformation("Received event {0}", message.Id);
        return Task.CompletedTask;
    }
}