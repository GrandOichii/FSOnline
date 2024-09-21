namespace FSLogsCollector.Settings;

public class RabbitMQSettings {
    public required string HostName { get; set; }
    public required int Port { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }

    public required string Exchange { get; set; }
    public required string Queue { get; set; }
}