using System.Text.Json.Serialization;

namespace FSLogsCollector;

public class LoggedMessage {
    public required DateTime Timestamp { get; set; }
    public required string MessageTemplate { get; set; }
    public required string Level { get; set; }
}