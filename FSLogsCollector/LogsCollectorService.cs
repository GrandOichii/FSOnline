using System.Text;
using FSLogsCollector.Services;
using FSLogsCollector.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FSLogsCollector;

public class LogsCollectorService : BackgroundService
{
    private readonly RabbitMQSettings _rmqSettings;
    private readonly ILogger<LogsCollectorService> _logger;
    private readonly ILoggedMessageProcessor _processor;
    private readonly IModel _channel;

    public LogsCollectorService(ILogger<LogsCollectorService> logger, IOptions<RabbitMQSettings> rmqSettings, ILoggedMessageProcessor processor) {
        _rmqSettings = rmqSettings.Value;
        var factory = new ConnectionFactory {
            HostName = _rmqSettings.HostName,
            Port = _rmqSettings.Port,
            UserName = _rmqSettings.UserName,
            Password = _rmqSettings.Password,
        };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
        _channel.QueueDeclare(_rmqSettings.Queue, true, false, false);
        _channel.QueueBind(_rmqSettings.Queue, _rmqSettings.Exchange, "");
        
        _logger = logger;
        _processor = processor;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        _logger.LogInformation("Started {ServiceName}", nameof(LogsCollectorService));

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (channel, args) => {
            _logger.LogInformation("Received tag: {Tag}", args.DeliveryTag);
            
            // uncomment to acknowledge
            // _channel.BasicAck(args.DeliveryTag, false);
            await _processor.Process(args.Body.ToArray());
        };
        _channel.BasicConsume(_rmqSettings.Queue, false, consumer);
    }
}