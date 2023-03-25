using System.Diagnostics;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace SampleDotNetOTEL.ProxyService.ExternalServices;

public sealed class MessageBroker : IDisposable
{
    public static readonly string TraceActivityName = typeof(MessageBroker).FullName!;
    private static readonly ActivitySource TraceActivitySource = new (TraceActivityName);
    
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageBroker(
        IConfiguration configuration,
        IHostApplicationLifetime hostApplicationLifetime)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(configuration.GetConnectionString("RabbitMQ")!)
        };
        
        while (!hostApplicationLifetime.ApplicationStopping.IsCancellationRequested)
        {
            try
            {
                _connection = factory.CreateConnection();
                break;
            }
            catch (BrokerUnreachableException)
            {
                // Ignore
            }
        }
        
        hostApplicationLifetime.ApplicationStopping.ThrowIfCancellationRequested();

        _channel = _connection!.CreateModel();
        
        _channel.QueueDeclare(
            queue: "test-messages",
            durable: false,
            exclusive: false,
            autoDelete: false
        );
    }

    public void PublishMessage(string message)
    {
        using var activity = TraceActivitySource.StartActivity(nameof(PublishMessage), ActivityKind.Producer);

        var basicProperties = _channel.CreateBasicProperties();

        if (activity?.Id != null)
        {
            basicProperties.Headers = new Dictionary<string, object>
            {
                { "traceparent", activity.Id }
            };
        }

        _channel.BasicPublish(
            exchange: string.Empty,
            routingKey: "test-messages",
            basicProperties: basicProperties, 
            body: Encoding.UTF8.GetBytes(message));
    }
    
    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}