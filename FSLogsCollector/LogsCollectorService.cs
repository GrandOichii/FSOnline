using System.Text;
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
    private readonly IModel _channel;

    public LogsCollectorService(ILogger<LogsCollectorService> logger, IOptions<RabbitMQSettings> rmqSettings) {
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
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        _logger.LogInformation("Started {ServiceName}", nameof(LogsCollectorService));

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (channel, args) => {
            var content = Encoding.UTF8.GetString(args.Body.ToArray());
            _logger.LogInformation("Received: {Message}", content);
            
            // uncomment to acknowledge
            // _channel.BasicAck(args.DeliveryTag, false);
        };
        _channel.BasicConsume(_rmqSettings.Queue, false, consumer);
    }
}