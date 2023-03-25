using System.Diagnostics;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace SampleDotNetOTEL.BusinessService.Workers;

public sealed class MessagesProcessingBackgroundService : BackgroundService
{
    public static readonly string TraceActivityName = typeof(MessagesProcessingBackgroundService).FullName!;
    private static readonly ActivitySource TraceActivitySource = new (TraceActivityName);

    private readonly ILogger<MessagesProcessingBackgroundService> _logger;
    
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessagesProcessingBackgroundService(
        IConfiguration configuration,
        IHostApplicationLifetime hostApplicationLifetime,
        ILogger<MessagesProcessingBackgroundService> logger)
    {
        _logger = logger;

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
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (_, e) => ProcessMessage(e);
        
        _channel.BasicConsume(
            queue: "test-messages",
            autoAck: true,
            consumer: consumer);

        return Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private void ProcessMessage(BasicDeliverEventArgs e)
    {
        string? parentActivityId = null;
        if (e.BasicProperties?.Headers?.TryGetValue("traceparent", out var parentActivityIdRaw) == true &&
            parentActivityIdRaw is byte[] traceParentBytes)
            parentActivityId = Encoding.UTF8.GetString(traceParentBytes);

        using var activity = TraceActivitySource.StartActivity(nameof(ProcessMessage), kind: ActivityKind.Consumer, parentId: parentActivityId);

        var body = e.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        _logger.LogInformation("Received message: {Message}", message);
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        await base.StopAsync(stoppingToken);

        _channel.Close();
        _connection.Close();
    }

    public override void Dispose()
    {
        base.Dispose();
        
        _channel.Dispose();
        _connection.Dispose();
    }
}