using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace FSLogsCollector.Services;

public class LoggedMessageLogger(ILogger<LoggedMessageLogger> logger) : ILoggedMessageProcessor {
    private readonly ILogger<LoggedMessageLogger> _logger = logger;

    public async Task Process(byte[] message) {
        var lm = JsonSerializer.Deserialize<LoggedMessage>(message)
            ?? throw new ArgumentException($"Failed to deserialize {nameof(LoggedMessage)}");

        _logger.LogInformation("Received message: {MessageTemplate}", lm.MessageTemplate);
    }
}